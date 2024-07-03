package userdatamanager

import (
	"encoding/json"
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

func CloseDB() {
	db.Close()
}
func GetGDKeysFromUDKey(udkey string) []string {
	var existingGdKeys []string
	formattedKey := fmt.Sprintf("%s::gdkeys", udkey)
	db.View(func(txn *badger.Txn) error {
		gdkeys, err := txn.Get([]byte(formattedKey))
		if err == nil {
			err = gdkeys.Value(func(val []byte) error {
				json.Unmarshal(val, &existingGdKeys)
				return nil
			})
			if err != nil {
				fmt.Println(err)
				return err
			}
		}
		return nil
	})
	return existingGdKeys
}
func AddGDKeyToUDKey(udkey string, gdkey string) {
	formattedKey := fmt.Sprintf("%s::gdkeys", udkey)
	existingGdKeys := GetGDKeysFromUDKey(udkey)
	existingGdKeys = append(existingGdKeys, gdkey)
	err := db.Update(func(txn *badger.Txn) error {
		serializedGdkeys, _ := json.Marshal(existingGdKeys)
		err := txn.Set([]byte(formattedKey), serializedGdkeys)
		return err
	})
	if err != nil {
		log.Fatal(err)
	}
}
