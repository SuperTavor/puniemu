import json
import os
import aiohttp
import asyncio

async def download_file(session, url, folder, filename, use_server_structure):
    if use_server_structure:
        folder_path = os.path.join(download_directory, folder)
    else:
        folder_path = os.path.join(download_directory, os.path.basename(url))
    
    os.makedirs(folder_path, exist_ok=True)
    file_path = os.path.join(folder_path, filename)
    
    try:
        async with session.get(url) as response:
            if response.status == 200:
                with open(file_path, 'wb') as f:
                    f.write(await response.read())
                print(f"Downloaded: {file_path}")
            else:
                print(f"Failed to download {url}: Status {response.status}")
    except Exception as e:
        print(f"Error downloading {url}: {e}")

async def download_files(json_file_path, download_directory, use_server_structure):
    with open(json_file_path, 'r', encoding='utf-8') as f:
        data = json.load(f)

    urls = data.get("urls", [])

    async with aiohttp.ClientSession() as session:
        tasks = []
        for file_info in urls:
            folder = file_info["Folder"]
            filename = file_info["Filename"]
            url = file_info["url"]
            tasks.append(download_file(session, url, folder, filename, use_server_structure))

        await asyncio.gather(*tasks)

if __name__ == "__main__":
    use_server_structure_input = input("Use server structure (y/n): ").strip().lower()
    use_server_structure = use_server_structure_input == 'y'

    json_file_path = input("Path to data download db: ").strip()
    download_directory = input("Output directory: ").strip()

    asyncio.run(download_files(json_file_path, download_directory, use_server_structure))
