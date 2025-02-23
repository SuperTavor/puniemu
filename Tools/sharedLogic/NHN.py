import hashlib
from base64 import urlsafe_b64encode, b64decode
from gzip import decompress
from Cryptodome.Util.Padding import unpad, pad
from Cryptodome.Cipher import AES

KEY = bytes([0xa8, 0x65, 0xd7, 0xe5, 0xe2, 0x45, 0x8f, 0x8c, 0xe1, 0xb5, 0xec, 0xd0, 0x87, 0xe5, 0x45, 0x94])
GAMESERVER = "https://gameserver.yw-p.com/"

headers = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    "Accept-Encoding": "identity",
    "Connection": "Keep-Alive",
    "User-Agent": "Dalvik/2.1.0 (Linux; U; Android 9; SM-G965N Build/QP1A.190711.020)"
}

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
    return urlsafe_b64encode(encrypted)

def calc_digest(payload: bytes) -> bytes:
    SALT = b"0bk2kvtFE2"
    sha1 = hashlib.sha1()
    sha1.update(SALT + b' ' + payload)
    digest = sha1.digest()
    secondSha1 = hashlib.sha1()
    secondSha1.update(SALT + digest)
    return secondSha1.digest()

