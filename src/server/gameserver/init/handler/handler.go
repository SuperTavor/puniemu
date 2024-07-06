package handler

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	var request map[string]any = make(map[string]any)
	var err error
	var decodedRequest string
	var encryptedRequest []byte
	encryptedRequest, err = io.ReadAll(r.Body)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	decodedRequest, err = nhnrequests.DecodeAndDecrypt(string(encryptedRequest[:]))
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	err = json.Unmarshal([]byte(decodedRequest), &request)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	if configmanager.CurrentConfig.MatchingGameVersion != fmt.Sprintf("%v", request["appVer"]) {
		res := gameServerModels.NewMsgAndBackToTitle("Your client version doesn't match\nthe one stored on the Puniemu server.", "Puniemu")
		resJson, _ := json.Marshal(res)
		encrypted, _ := nhnrequests.EncryptAndEncode(string(resJson[:]))
		w.Write([]byte(encrypted))
		return
	}
	var outputJson map[string]any = make(map[string]any)
	outputJson["serverDt"] = time.Now().Unix()
	outputJson["mstVersionMaster"] = configmanager.CurrentConfig.MstVersionMaster
	outputJson["resultCode"] = 0
	outputJson["nextScreenType"] = 0
	outputJson["ywp_mst_version_master"] = configmanager.StaticJsons["ywp_mst_version_master"]
	//Deserialize hitodamaShopSaleList from static-jsons into a slice.
	var hitodamaShopSaleList []int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["hitodamaShopSaleList"]), &hitodamaShopSaleList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	outputJson["hitodamaShopSaleList"] = hitodamaShopSaleList
	outputJson["gameServerUrl"] = configmanager.CurrentConfig.OgGameServerURL
	outputJson["storeUrl"] = ""
	outputJson["isEnableSerialCode"] = 1
	outputJson["apkey"] = configmanager.CurrentConfig.L5IDEmulatedAPIKey
	outputJson["imgServer"] = "dataDownload/"
	outputJson["resultType"] = 0
	outputJson["dispNoticeFlg"] = 2
	var shopSaleList []int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["shopSaleList"]), &shopSaleList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	outputJson["shopSaleList"] = shopSaleList
	outputJson["ywpToken"] = ""
	var ymoneyShopSaleList []int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["ymoneyShopSaleList"]), &ymoneyShopSaleList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	var noticePageList []map[string]int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["noticePageList"]), &noticePageList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	outputJson["noticePageList"] = noticePageList
	outputJson["ymoneyShopSaleList"] = ymoneyShopSaleList
	outputJson["l5idUrl"] = "l5id"
	outputJson["isAppleTrial"] = false
	outputJson["isEnableFriendInvite"] = 1
	outputJson["masterReacquisitionHour"] = 2
	outputJson["isEnableYoukaiMedal"] = 1
	//We don't support linking.
	outputJson["isEnableL5ID"] = 0
	outputJson["threeKingdomTeamEventButtonHiddenFlg"] = 1
	outputJson["teamEventButtonHiddenFlg"] = 1
	outputJson["dialogMsg"] = ""
	outputJson["webServerIp"] = ""
	outputJson["token"] = ""
	resJson, _ := json.Marshal(outputJson)
	encryptedResJson, _ := nhnrequests.EncryptAndEncode(string(resJson[:]))
	w.Write([]byte(encryptedResJson))
}
