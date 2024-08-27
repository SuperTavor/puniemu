
# Mostly written by DarkCraft, had to be commited by me (Zura) unfortunately
import json
from sharedLogic import NHN

SERVER = "https://ywp-down.hangame.co.jp/eal/"
def get_urls(data, url):
    return [{"Folder": x[2], "Filename": x[1], "url": f"{url}/{x[5]}/{x[3]}/{x[1]}"} 
            for x in [item.split('|') for item in data.split("*")]]

while True:
    try:
        md = input("Enter master data response: ").strip()
        data = json.loads(NHN.decrypt_res(md))["ywp_mst_version_resource"]

        output_path = input("Enter output file path: ").strip().replace("'", "").replace('"', '')
        open(output_path, "w").close() 

        urls = {}
        urls["urls"] = get_urls(data["tableData"],SERVER )
        urls["version"] = data["version"]

        with open(output_path, 'w') as f:
            json.dump(urls, f, indent=4)

        print(f"Written to '{output_path}'")
        break

    except Exception as e:
        print(f"Error: {e}")

input("Press any button to exit")
