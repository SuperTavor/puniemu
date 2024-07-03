package models

import (
	"crypto/md5"
	"encoding/hex"
	"fmt"
	"math"
	"math/rand"
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
	randomNumber := rand.Intn(math.MaxInt32 + 1)
	randomString := fmt.Sprintf("%d", randomNumber)
	//Add random onto key based on
	combined := baseOn + randomString
	//Hash
	hash := md5.Sum([]byte(combined))
	generatedKey := prefix + hex.EncodeToString(hash[:])
	return generatedKey
}
