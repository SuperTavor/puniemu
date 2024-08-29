import json
import os
import aiohttp
import asyncio
import requests
from urllib.parse import urlparse
from sharedLogic import NHN

DL_SRV = "https://ywp-down.hangame.co.jp/eal"
use_game_structure = False
async def download_file(session, data, download_directory):
    if use_game_structure:
        path = data["folder"] + '/' + data['fname']
    else: path = urlparse(data["url"]).path.lstrip('/')
    file_path = os.path.join(download_directory, path)
    
    os.makedirs(os.path.dirname(file_path), exist_ok=True)
    
    async with session.get(data["url"]) as response:
        if response.status == 200:
            with open(file_path, 'wb') as f:
                f.write(await response.read())
            print(f"Downloaded: {file_path}")

async def download_files(download_directory, urls):
    async with aiohttp.ClientSession() as session:
        tasks = [download_file(session, url, download_directory) for url in urls]
        await asyncio.gather(*tasks)

def get_installation_data(data, url):
    return [{"folder": x[2], "fname": x[1], "url": f"{url}/{x[5]}/{x[3]}/{x[1]}"} 
            for x in [item.split('|') for item in data.split("*")]]
def gather_urls():
    data = NHN.encrypt_req(f'''{{
        "appVer": "{input("Enter latest game version: ")}",
        "deviceId": "d-822a61c56b60d03dd373aab9826210e1",
        "level5UserId": "0",
        "osType": 2,
        "tableNames": "ywp_mst_version_resource",
        "userId": "0",
        "ywpToken": "0"
        }}''')
    md = requests.post(NHN.GAMESERVER+"getMaster.nhn", headers=NHN.headers, data=data).text
    data = json.loads(NHN.decrypt_res(md))["ywp_mst_version_resource"]
    idata = get_installation_data(data["tableData"],DL_SRV)
    return idata
    
if __name__ == "__main__":
    use_game_structure = input("Use game file structure? ") == 'y'

    download_directory = input("Output directory: ").strip()
    asyncio.run(download_files(download_directory, gather_urls()))
