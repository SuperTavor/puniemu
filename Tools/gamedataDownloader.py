import requests
import json
import os
from sharedLogic.NHN import *

output_folder = input("Output folder: ")
appver = input("Game version to spoof (latest version only): ").strip()

initReqPayload = f'{{"appVer":"{appver}","deviceId":"0","level5UserId":"0","mstVersionVer":0,"osType":2,"signature":"s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=","userId":"0","ywpToken":"0"}}'
masterReqPayload = f'{{"appVer":"{appver}","deviceId":"d-50a6781418815598cb80657362959a0943684146efb6da8358870bef8c34d3e0","level5UserId":"0","mstVersionVer":0,"osType":2,"tableNames":"all","userId":"0","ywpToken":"0"}}'
loginReqPayload = f'{{"appVer":"{appver}","batteryInfo":{{"level":85,"state":2,"technology":"Li-ion","temperature":328,"voltage":3500}},"deviceId":"d-k3l6lh7d0jtafxq3ez5twlf5t1c4mcbwx905t6y8qwg50siubxj2yrkriq2lhf","deviceName":"z3q","gdkeySignature":"4c794e61006327c2baa5c6945439b2797a052ce923c75132ad6264a5a16c93f5","gdkeyValue":"g-h7wm3eepmnyr1pnb2w1cz9zmzpqtyls1qr02ot6hkygz0siuct92yr792q2lhq","isL5IDLinked":0,"level5UserId":"g-h7wm3eepmnyr1pnb2w1cz9zmzpqtyls1qr02ot6hkygz0siuct92yr792q2lhq","modelName":"SM-G988N","mstVersionVer":16464,"osType":2,"osVersion":"9","signNonce":"74decdf4-e86d-4228-951e-b11ccd342a4d","signTimestamp":"1724703138","signature":"s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=","udkeySignature":"6801fe8fde40145938be564c0a79c7d45b996d4da3ac7b03fdf9b2c3e4901bd9","udkeyValue":"d-k3l6lh7d0jtafxq3ez5twlf5t1c4mcbwx905t6y8qwg50siubxj2yrkriq2lhf","userId":"3347321526146","ywpToken":"0"}}'


try:
    #Send message to init.nhn
    encryptedInitRes = requests.post(GAMESERVER+"init.nhn", headers=headers, data=encrypt_req(initReqPayload))
    init_dict = json.loads(decrypt_res(encryptedInitRes.text))
    os.makedirs(output_folder, exist_ok=True)
    files = {
        "ywp_mst_version_master": init_dict["ywp_mst_version_master"],
        "hitodamaShopSaleList": json.dumps(init_dict["hitodamaShopSaleList"],ensure_ascii=False),
        "shopSaleList": json.dumps(init_dict["shopSaleList"],ensure_ascii=False),
        "ymoneyShopSaleList": json.dumps(init_dict["ymoneyShopSaleList"],ensure_ascii=False),
        "noticePageList": json.dumps(init_dict["noticePageList"],ensure_ascii=False),
        "mstVersionMaster": json.dumps(init_dict["mstVersionMaster"],ensure_ascii=False)
    }
    for key,value in files.items():
    
        if key != "ywp_mst_version_master":
            json.dump(json.loads(value),open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8"),ensure_ascii=False)
        else:
            open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8").write(value)
    table_trigger=[]
    #Send message to getMaster
    encryptedMasterRes = requests.post(GAMESERVER+"getMaster.nhn",headers=headers,data=encrypt_req(masterReqPayload))
    master_dict = json.loads(decrypt_res(encryptedMasterRes.text))
    for key,value in master_dict.items():
        if "ywp_mst" in key:
            table_trigger.append(key)
            json.dump(value,open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8"),ensure_ascii=False)
    
    #send message to login
    encryptedLoginRes = requests.post(GAMESERVER+"login.nhn",headers=headers,data=encrypt_req(loginReqPayload))
    login_dict = json.loads(decrypt_res(encryptedLoginRes.text))
    for key,value in login_dict.items():
        if "ywp_mst" in key or key == "leaderYoukaiBGM" or key == "noticePageListFlg" or key == "teamEventButtonHiddenFlg" or key == "mstMapMobPeriodNoList" or key == "responseCodeTeamEvent":
            if key not in table_trigger:
                table_trigger.append(key)
                json.dump(value,open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8"),ensure_ascii=False)
        elif "ywp_user" in key:
            json.dump(value,open(os.path.join(output_folder, f"{key}_def.txt"), "w",encoding="utf-8"),ensure_ascii=False)
    """for key, value in files.items():
        with open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8") as f:
            f.write(value.replace("'", "\""))"""

    print("Finished")
except Exception as e:
    print(f"An error occurred: {e}")
