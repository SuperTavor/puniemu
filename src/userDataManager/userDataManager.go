package userdatamanager

import (
	"encoding/json"
	"fmt"
	"log"
	"time"

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
	initFriendCodesMap()
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

func AddGDKeyToUDKey(udkey string, gdkey string) error {
	formattedKey := fmt.Sprintf("%s::gdkeys", udkey)
	existingGdKeys := GetGDKeysFromUDKey(udkey)
	existingGdKeys = append(existingGdKeys, gdkey)
	err := db.Update(func(txn *badger.Txn) error {
		serializedGdkeys, _ := json.Marshal(existingGdKeys)
		err := txn.Set([]byte(formattedKey), serializedGdkeys)
		return err
	})
	return err
}

func StoreYwpUser(udkey, gdkey, tableName string, content []byte) error {
	err := db.Update(func(txn *badger.Txn) error {
		formattedKey := fmt.Sprintf("%s::%s::ywp_user::%s", udkey, gdkey, tableName)
		err := txn.Set([]byte(formattedKey), content)
		if err != nil {
			return err
		}
		return nil
	})
	return err
}

// We use a map so I can see if a friend code is already registered in o(n)
var friendCodes map[string]bool

const FRIEND_CODE_DB_KEY = "FRIEND_CODES"

func IsFriendCodeExists(friendCode string) bool {
	_, ok := friendCodes[friendCode]
	return ok
}
func initFriendCodesMap() {
	err := db.View(func(txn *badger.Txn) error {
		item, err := txn.Get([]byte(FRIEND_CODE_DB_KEY))
		if err != nil {
			friendCodes = make(map[string]bool)
		} else {
			item.Value(func(val []byte) error {
				return json.Unmarshal(val, &friendCodes)
			})
		}
		return nil
	})
	if err != nil {
		log.Fatal("Could not retrieve friend code list: " + err.Error())
	}
}
func StoreFriendCode(friendCode string) error {
	friendCodes[friendCode] = true
	err := db.Update(func(txn *badger.Txn) error {
		marshalledFriendCodes, err := json.Marshal(friendCodes)
		if err != nil {
			return err
		}
		err = txn.Set([]byte(FRIEND_CODE_DB_KEY), marshalledFriendCodes)
		if err != nil {
			return err
		}
		return nil
	})
	return err
}
func GetYwpUser(udkey, gdkey, tableName string) ([]byte, error) {
	var outputBytes []byte
	err := db.View(func(txn *badger.Txn) error {
		formattedKey := fmt.Sprintf("%s::%s::ywp_user::%s", udkey, gdkey, tableName)
		item, err := txn.Get([]byte(formattedKey))
		if err != nil {
			return err
		} else {
			item.Value(func(val []byte) error {
				outputBytes = val
				return nil
			})
			return nil
		}
	})
	return outputBytes, err
}

func UnixTimeToDate(timestamp int64) string {
	t := time.Unix(timestamp, 0)
	formattedTime := t.Format("2006-01-02 15:04:05")
	return formattedTime
}
