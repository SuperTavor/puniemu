package userdatamanager

import (
	"bytes"
	"encoding/gob"
	"fmt"
	"log"

	"github.com/dgraph-io/badger/v3"
)

var db *badger.DB

func InitializeDB() {
	opts := badger.DefaultOptions("./badger")
	var err error
	db, err = badger.Open(opts)
	if err != nil {
		log.Fatal(err)
	}
}
func serialize(item any) []byte {
	var buf bytes.Buffer
	encoder := gob.NewEncoder(&buf)
	encoder.Encode(item)
	bytes := buf.Bytes()

	return bytes
}

func deserialize[T any](bytesToDe []byte) T {
	var buf bytes.Buffer
	buf.Write(bytesToDe)

	dec := gob.NewDecoder(&buf)

	var item T
	err := dec.Decode(&item)
	if err != nil {
		log.Fatal("Decode error:", err)
	}

	return item
}
func CloseDB() {
	db.Close()
}

func AddGDKeyToUDKey(udkey string, gdkey string) {
	formattedKey := fmt.Sprintf("%s::gdkeys", udkey)
	var existingGdKeys []string
	db.View(func(txn *badger.Txn) error {
		gdkeys, err := txn.Get([]byte(formattedKey))
		if err == nil {
			err = gdkeys.Value(func(val []byte) error {
				existingGdKeys = deserialize[[]string](val)
				return nil
			})
			if err != nil {
				fmt.Println(err)
				return err
			}
		}
		return nil
	})
	existingGdKeys = append(existingGdKeys, gdkey)
	err := db.Update(func(txn *badger.Txn) error {
		err := txn.Set([]byte(formattedKey), serialize(existingGdKeys))
		return err
	})
	if err != nil {
		log.Fatal(err)
	}
}
