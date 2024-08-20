from base64 import b64decode, b64encode
from gzip import decompress
from Cryptodome.Cipher import AES
from Cryptodome.Util.Padding import unpad, pad
import hashlib
import os
import requests
import json

KEY = bytes([0xa8, 0x65, 0xd7, 0xe5, 0xe2, 0x45, 0x8f, 0x8c, 0xe1, 0xb5, 0xec, 0xd0, 0x87, 0xe5, 0x45, 0x94])

def decrypt_res(input_text: str) -> str:
    input_text = input_text.strip().replace('-', '+').replace('_', '/')
    input_text += "==" if not input_text.endswith("==") else ""
    decrypted = unpad(AES.new(KEY, AES.MODE_ECB).decrypt(b64decode(input_text)), AES.block_size)[20:]
    return decompress(decrypted).decode("utf-8")

def encrypt_req(to_encrypt: str) -> str:
    to_encrypt_bytes = to_encrypt.encode('utf-8')
    digest = calc_digest(to_encrypt_bytes)
    padded = pad(digest + to_encrypt_bytes, AES.block_size)
    encrypted = AES.new(KEY, AES.MODE_ECB).encrypt(padded)
    return b64encode(encrypted).decode().replace('+', '-').replace('/', '_')

def calc_digest(payload: bytes) -> bytes:
    SALT = b"0bk2kvtFE2"
    sha1 = hashlib.sha1()
    sha1.update(SALT + b' ' + payload)
    digest = sha1.digest()
    sha1.update(SALT + digest)
    return sha1.digest()

SERVER = "https://gameserver.yw-p.com/"
output_folder = input("Output folder: ")
appver = input("Game version to spoof (latest version only): ").strip()

init_request = json.dumps({
    "appVer": appver,
    "deviceId": "0",
    "level5UserId": "0",
    "mstVersionVer": 0,
    "osType": 2,
    "signature": "s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=",
    "userId": "0",
    "ywpToken": "0"
})

encrypted_res = requests.post(SERVER + "init.nhn", data=encrypt_req(init_request), headers={"Content-Type": "application/json"}).text
res_dict = json.loads(decrypt_res(encrypted_res))

files = {
    "ywp_mst_version_master": res_dict["ywp_mst_version_master"],
    "hitodamaShopSaleList": str(res_dict["hitodamaShopSaleList"]),
    "shopSaleList": str(res_dict["shopSaleList"]),
    "ymoneyShopSaleList": str(res_dict["ymoneyShopSaleList"]),
    "noticePageList": str(res_dict["noticePageList"])
}

os.makedirs(output_folder, exist_ok=True)

for key, value in files.items():
    with open(os.path.join(output_folder, f"{key}.txt"), "w") as f:
        f.write(value.replace("'", "\""))

print("Finished")
