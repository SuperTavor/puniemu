package models

import (
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"

	l5idapi_models "github.com/SuperTavor/Puniemu/src/server/l5idapi/models"
)

type CreateGDKey_GoodResponse struct {
	//Result is true if response is good.
	Result bool `json:"result"`
	//We generate this key, view more info about it in keyset.go.
	GDKey         l5idapi_models.Key `json:"gdkey"`
	SignNonce     string             `json:"sign_nonce"`
	SignTimestamp int                `json:"sign_timestamp"`
}

func NewCreateGDKeyGoodResponse(udkey string) CreateGDKey_GoodResponse {
	generatedGdkey := l5idapi_models.NewKeyAutoGen(udkey, "g-")
	addCorrelationToKeyMap(udkey, generatedGdkey)
	return CreateGDKey_GoodResponse{Result: true, GDKey: l5idapi_models.NewKey(generatedGdkey), SignNonce: "123", SignTimestamp: int(time.Now().Unix())}
}
func addCorrelationToKeyMap(udkey string, gdkey string) {
	if _, ok := configmanager.KeyMap[udkey]; ok {
		configmanager.KeyMap[udkey] = append(configmanager.KeyMap[udkey], gdkey)
	} else {
		configmanager.KeyMap[udkey] = []string{gdkey}
	}
}
