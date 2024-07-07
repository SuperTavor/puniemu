package models

import (
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
)

type InitResponse struct {
	//timestamp when the response was sent
	ServerDt int64 `json:"serverDt"`
	//Version of assets on the server
	MstVersionMaster int `json:"mstVersionMaster"`
	//0 here
	ResultCode int `json:"resultCode"`
	//0 here
	NextScreenType int `json:"nextScreenType"`
	//Don't know, but is a constant.
	YwpMstVersionMaster string `json:"ywp_mst_version_master"`
	//Constant.
	HitodamaShopSaleList []int `json:"hitodamaShopSaleList"`
	//URL of the OG server.
	GameServerURL string `json:"gameServerUrl"`
	//URL to the google play store if an updated is needed. Since this response is intended as a valid response, this will be empty.
	StoreURL string `json:"storeURL"`
	//1 here.
	IsEnableSerialCode int `json:"isEnableSerialCode"`
	//Level5 ID API emulated api key.
	APKey string `json:"apkey"`
	//URL for the data download. Since we wanna download from the gameserver's `/datadownload` path, we simply use /datadownload.
	ImgServer string `json:"imgServer"`
	//0 here.
	ResultType int `json:"resultType"`
	//2 here.
	DispNoticeFlag int `json:"dispNoticeFlag"`
	//Constant.
	ShopSaleList []int `json:"shopSaleList"`
	//Empty here
	YwpToken string `json:"ywpToken"`
	//Constant.
	YMoneyShopSaleList []int `json:"ymoneyShopSaleList"`
	//Constant.
	NoticePageList []map[string]int `json:"noticePageList"`
	//Same deal as the data download URL. we just put /l5id to point it to the gameserver's l5id path.
	L5IDURL string `json:"l5idUrl"`
	//False here.
	IsAppleTrial bool `json:"isAppleTrial"`
	//1 here.
	IsEnableFriendInvite int `json:"isEnableFriendInvite"`
	//2 here
	MasterReacquisitionHour int `json:"masterReacquisitionHour"`
	//1 here
	IsEnableYokaiMedal int `json:"isEnableYoukaiMedal"`
	//0 here.
	IsEnableL5ID int `json:"isEnableL5ID"`
	//1 here.
	ThreeKingdomTeamEventButtonHiddenFlg int `json:"threeKingdomTeamEventButtonHiddenFlg"`
	//1 here.
	TeamEventButtonHiddenFlg int `json:"teamEventButtonHiddenFlg"`
}

func NewInitResponse(hitodamaShopSaleList, shopSaleList, ymoneyShopSaleList []int, noticePageList []map[string]int) *InitResponse {
	return &InitResponse{
		ServerDt:                             time.Now().Unix(),
		MstVersionMaster:                     configmanager.CurrentConfig.MstVersionMaster,
		ResultCode:                           0,
		NextScreenType:                       0,
		YwpMstVersionMaster:                  configmanager.StaticJsons["ywp_mst_version_master"],
		HitodamaShopSaleList:                 hitodamaShopSaleList,
		GameServerURL:                        configmanager.CurrentConfig.OgGameServerURL,
		StoreURL:                             "",
		IsEnableSerialCode:                   1,
		APKey:                                configmanager.CurrentConfig.L5IDEmulatedAPIKey,
		ImgServer:                            "dataDownload/",
		ResultType:                           0,
		DispNoticeFlag:                       2,
		ShopSaleList:                         shopSaleList,
		YwpToken:                             "",
		YMoneyShopSaleList:                   ymoneyShopSaleList,
		NoticePageList:                       noticePageList,
		L5IDURL:                              "l5id",
		IsAppleTrial:                         false,
		IsEnableFriendInvite:                 1,
		MasterReacquisitionHour:              2,
		IsEnableYokaiMedal:                   1,
		IsEnableL5ID:                         0,
		ThreeKingdomTeamEventButtonHiddenFlg: 1,
		TeamEventButtonHiddenFlg:             1,
	}
}
