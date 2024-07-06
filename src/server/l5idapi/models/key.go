package models

import (
	"crypto/md5"
	"encoding/hex"
	"time"

	"strconv"
)

type Key struct {
	Value     string `json:"value"`
	Signature string `json:"signature"`
}

func NewKey(keyValue string) Key {
	k := Key{
		Value: keyValue,
		//Game doesn't seem to care about sig so it can be empty
		Signature: "",
	}
	return k
}

func NewKeyAutoGen(baseOn string, prefix string) string {
	timeRightNow := time.Now().UnixMicro()
	//Add time onto key based on
	combined := baseOn + strconv.FormatInt(timeRightNow, 10)
	//Hash
	hash := md5.Sum([]byte(combined))
	generatedKey := prefix + hex.EncodeToString(hash[:])
	return generatedKey
}
