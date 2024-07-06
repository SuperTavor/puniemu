package models

import (
	"encoding/json"
	"log"
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
)

type CreateUserResponse struct {
	//Constant.
	ShopSaleList []int `json:"shopSaleList"`
	//The unix time when the response was sent
	ServerDate int64 `json:"serverDt"`
	//Always empty.
	YwpToken string `json:"ywpToken"`
	//Constant.
	YMoneyShopSaleList []int `json:"ymoneyShopSaleList"`
	//Version of assets/data on the server
	MstVersionMaster int `json:"mstVersionMaster"`
	//Empty.
	RewardList []int `json:"rewardList"`
	//0 here
	ResultCode int `json:"resultCode"`
	//Table that dictates which tutorial flags the user has completed
	UserTutorialList string `json:"ywp_user_tutorial_list"`
	//0 here.
	NextScreenType int `json:"nextScreenType"`
	//Constant.
	HitodamaShopSaleList []int `json:"hitodamaShopSaleList"`
	//Basic user data.
	UserData gameServerModels.YwpUserData `json:"ywp_user_data"`
	//0 here.
	ResultType int `json:"resultType"`
}

func NewCreateUserResponse(userTutorialList string, userData gameServerModels.YwpUserData) *CreateUserResponse {
	var shopSaleList []int
	var ymoneyShopSaleList []int
	var hitodamaShopSaleList []int
	err := json.Unmarshal([]byte(configmanager.StaticJsons["shopSaleList"]), &shopSaleList)
	if err != nil {
		log.Println(err)
		return nil
	}
	err = json.Unmarshal([]byte(configmanager.StaticJsons["ymoneyShopSaleList"]), &ymoneyShopSaleList)
	if err != nil {
		log.Println(err)
		return nil
	}
	err = json.Unmarshal([]byte(configmanager.StaticJsons["hitodamaShopSaleList"]), &hitodamaShopSaleList)
	if err != nil {
		log.Println(err)
		return nil
	}
	out := CreateUserResponse{
		ShopSaleList:         shopSaleList,
		ServerDate:           time.Now().Unix(),
		YwpToken:             "",
		YMoneyShopSaleList:   ymoneyShopSaleList,
		MstVersionMaster:     configmanager.CurrentConfig.MstVersionMaster,
		RewardList:           make([]int, 0),
		ResultCode:           0,
		UserTutorialList:     userTutorialList,
		NextScreenType:       0,
		HitodamaShopSaleList: hitodamaShopSaleList,
		UserData:             userData,
		ResultType:           0,
	}
	return &out
}
