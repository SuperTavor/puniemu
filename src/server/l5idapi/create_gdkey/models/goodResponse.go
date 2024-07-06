package models

import (
	"log"
	"time"

	userdatamanager "github.com/SuperTavor/Puniemu/src/userDataManager"

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

func NewCreateGDKeyGoodResponse(udkey string) (CreateGDKey_GoodResponse, error) {
	var generatedGdkey string = l5idapi_models.NewKeyAutoGen(udkey, "g-")
	err := userdatamanager.AddGDKeyToUDKey(udkey, generatedGdkey)
	if err != nil {
		log.Println(err)
		return CreateGDKey_GoodResponse{}, err
	}
	return CreateGDKey_GoodResponse{Result: true, GDKey: l5idapi_models.NewKey(generatedGdkey), SignNonce: "123", SignTimestamp: int(time.Now().Unix())}, nil
}
