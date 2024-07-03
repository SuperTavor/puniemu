package nhnrequests

import (
	"bytes"
	"compress/gzip"
	"crypto/aes"
	"crypto/sha1"
	"encoding/base64"
	"encoding/hex"
	"log"

	"github.com/andreburgaud/crypt2go/ecb"
)

func pad(data []byte, blockSize int) []byte {
	padding := blockSize - len(data)%blockSize
	padtext := bytes.Repeat([]byte{byte(padding)}, padding)
	return append(data, padtext...)
}
func unpad(data []byte) []byte {
	length := len(data)
	unpadding := int(data[length-1])
	return data[:(length - unpadding)]
}
func EncryptAndEncode(jsonPayload string) (string, error) {

	compressedJson := compressJson(jsonPayload)
	jsonDigest := calcDigest(compressedJson)
	var buf bytes.Buffer
	buf.Write(jsonDigest)
	buf.Write(compressedJson)
	decodedKey, _ := hex.DecodeString(HttpKey)
	encryptedBytes := encrypt(buf.Bytes(), decodedKey)

	encoded := base64.URLEncoding.EncodeToString(encryptedBytes)
	return encoded, nil
}
func compressJson(json string) []byte {
	var buf bytes.Buffer
	gz := gzip.NewWriter(&buf)
	gz.Write([]byte(json))
	gz.Close()
	return buf.Bytes()
}
func encrypt(plaintext, key []byte) []byte {
	block, err := aes.NewCipher(key)
	if err != nil {
		log.Println(err)
		return nil
	}
	mode := ecb.NewECBEncrypter(block)
	paddedPlaintext := pad(plaintext, aes.BlockSize)
	ciphertext := make([]byte, len(paddedPlaintext))
	mode.CryptBlocks(ciphertext, paddedPlaintext)
	return ciphertext
}

func decrypt(ciphertext, key []byte) []byte {
	block, err := aes.NewCipher(key)
	if err != nil {
		log.Println(err)
		return nil
	}
	mode := ecb.NewECBDecrypter(block)
	plaintext := make([]byte, len(ciphertext))
	mode.CryptBlocks(plaintext, ciphertext)
	return unpad(plaintext)
}
func calcDigest(compressedJson []byte) []byte {
	hash := func(data []byte) []byte {
		h := sha1.New()
		h.Write([]byte(data))
		return h.Sum(nil)
	}

	firstDigest := hash(append(append([]byte(DigestSalt), byte(' ')), compressedJson...))
	finalDigest := hash(append([]byte(DigestSalt), firstDigest...))

	return finalDigest
}

func DecodeAndDecrypt(base64Payload string) (outputJson string, err error) {
	decodedBytes, err := base64.URLEncoding.DecodeString(base64Payload)
	if err != nil {
		return "", err
	}

	keyBytes, _ := hex.DecodeString(HttpKey)

	decryptedBytes := decrypt(decodedBytes, keyBytes)
	//Remove first 20 bytes (checksum)
	decryptedBytes = decryptedBytes[20:]
	decryptedString := string(decryptedBytes[:])
	return decryptedString, nil
}
