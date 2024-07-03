package models

import (
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	l5idapi_models "github.com/SuperTavor/Puniemu/src/server/l5idapi/models"
)

type Active_GoodResponse struct {
	//True if response is good.
	Result bool `json:"result"`
	//Contains some keys. Look in the KeySet struct for more information about them.
	Keys []KeySet `json:"keys"`
	//The same keys repeated, because sometimes the real API returns them wraped in a KeySet and sometimes not, and the request is identical. Delivering both should mitigate any issues
	UnwrapedUDKEY  l5idapi_models.Key   `json:"udkey"`
	UnwrapedGDKeys []l5idapi_models.Key `json:"gdkeys"`

	//Checks if linked to a LEVEL5 ID. This will always be false in our case, since Puniemu doesn't support LEVEL5 IDs at the moment.
	IsLinked bool `json:"is_linked"`
	//Always 3.
	MaxGDKeys int `json:"max_gdkeys"`
	//I don't know what this does, but setting both variables to an empty string appears to work.
	RCClientVersion RCClientVersion `json:"rc_client_version"`
	//Account creation time in unix time
	SignTimestamp int `json:"sign_timestamp"`
	//I don't know what this does either, but setting it to anything works
	SignNonce string `json:"sign_nonce"`
}

func NewActiveGoodResponse(udkeyValue string) Active_GoodResponse {
	var gdkeys []l5idapi_models.Key
	for _, v := range retrieveGdkeysFromUdkey(udkeyValue) {
		gdkeys = append(gdkeys, l5idapi_models.NewKey(v))
	}
	if gdkeys == nil {
		gdkeys = make([]l5idapi_models.Key, 0)
	}
	var keyset KeySet
	keyset.GDKeys = gdkeys
	keyset.UDKey = l5idapi_models.NewKey(udkeyValue)
	var keysetList []KeySet
	keysetList = append(keysetList, keyset)
	obj := Active_GoodResponse{
		Result:          true,
		Keys:            keysetList,
		UnwrapedUDKEY:   l5idapi_models.NewKey(udkeyValue),
		UnwrapedGDKeys:  gdkeys,
		IsLinked:        false,
		MaxGDKeys:       3,
		RCClientVersion: NewRCClientVersion(),
		SignTimestamp:   int(time.Now().Unix()),
		SignNonce:       "123",
	}

	return obj
}

func retrieveGdkeysFromUdkey(udkeyValue string) []string {
	return configmanager.KeyMap[udkeyValue]
}
