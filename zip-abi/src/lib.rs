use std::ffi::{CStr, c_char};
use std::fs;
use std::fs::File;
use std::io::{copy, Read, Seek, Write};
use std::path::Path;
use std::str;

use walkdir::{DirEntry, WalkDir};
use zip::write::FileOptions;
use zip::CompressionMethod;
#[no_mangle]
pub extern "C" fn compress_dir(src_dir: * const c_char, target:* const c_char, method:i32) -> i32 {
    let src=mkstr(src_dir);
    let src_path=Path::new(src.as_str());
    let target_str=mkstr(target);
    let target_path=Path::new(target_str.as_str());
    let zipfile = std::fs::File::create(target_path).unwrap();
    let dir: WalkDir = WalkDir::new(src_path);
    zip_dir(
        &mut dir.into_iter().filter_map(|e| e.ok()),
        src.as_str(),
        zipfile,
        method
    )
    .unwrap_or(());
    1
}

fn mkstr(s: *const c_char) -> String {
    let c_str = unsafe {
        assert!(!s.is_null());
        CStr::from_ptr(s)
    };
 
    let r_str = c_str.to_str().expect("Could not successfully convert string form foreign code!");
    String::from(r_str)
}



fn zip_dir<T>(
    it: &mut dyn Iterator<Item = DirEntry>,
    prefix: &str,
    writer: T,
    method:i32,
) -> zip::result::ZipResult<()>
where
    T: Write + Seek,
{
    let compressMethod:CompressionMethod= match method {
        0=>CompressionMethod::Stored,
        1|_=>CompressionMethod::Deflated,
        2=>CompressionMethod::Bzip2,
        3=>CompressionMethod::Aes,
        4=>CompressionMethod::Zstd,
    };

    let mut zip = zip::ZipWriter::new(writer);
    let options = FileOptions::default()
        .compression_method(compressMethod) 
        .unix_permissions(0o755); 

    let mut buffer = Vec::new();
    for entry in it {
        let path = entry.path();
       
        let name = path.strip_prefix(Path::new(prefix)).unwrap();
        
        
        if path.is_file() {
           
            zip.start_file_from_path(name.into(), options)?;
            let mut f = File::open(path)?;

            f.read_to_end(&mut buffer)?;
            zip.write_all(&*buffer)?;
            buffer.clear();
        } else if name.as_os_str().len() != 0 {
           
            zip.add_directory_from_path(name, options)?;
        }
    }
    zip.finish()?;
    Result::Ok(())
}

#[no_mangle]
pub extern "C" fn extract_file(src: * const c_char, target: * const c_char)->i32 {
    let src_str=mkstr(src);
    let target_str=mkstr(target);
    let src_path=Path::new(src_str.as_str());
    let target_path=Path::new(target_str.as_str());
    let zipfile = std::fs::File::open(src_path).unwrap();
    
    let mut zip = zip::ZipArchive::new(zipfile).unwrap();

    if !target_path.exists() {
        fs::create_dir_all(target_path).unwrap_or_default();
       
    }
    for i in 0..zip.len() {
        let mut file = zip.by_index(i).unwrap();
        println!("Filename: {} {:?}", file.name(), file.sanitized_name());
        if file.is_dir() {
            
            let target = target_path.join(Path::new(&file.name().replace("\\", "")));
            fs::create_dir_all(target).unwrap();
        } else {
            let file_path = target_path.join(Path::new(file.name()));
            let mut target_file = if !file_path.exists() {
                println!("file path {}", file_path.to_str().unwrap());
                fs::File::create(file_path).unwrap()
            } else {
                fs::File::open(file_path).unwrap()
            };
            copy(&mut file, &mut target_file).unwrap_or_default();
        }
    }
    1
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn it_works() {}
}
