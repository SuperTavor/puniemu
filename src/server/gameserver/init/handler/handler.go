package handler

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
	models "github.com/SuperTavor/Puniemu/src/server/gameserver/init/models"
	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Only POST is supported for this path.", http.StatusBadRequest)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	var request models.InitRequest
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
	if configmanager.CurrentConfig.MatchingGameVersion != fmt.Sprintf("%v", request.AppVer) {
		res := gameServerModels.NewMsgAndBackToTitle("Your client version doesn't match\nthe one stored on the Puniemu server.", "Puniemu")
		resJson, _ := json.Marshal(res)
		encrypted, _ := nhnrequests.EncryptAndEncode(string(resJson[:]))
		w.Write([]byte(encrypted))
		return
	}
	//Deserialize some constants from static-jsons into objects for the response.
	var hitodamaShopSaleList []int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["hitodamaShopSaleList"]), &hitodamaShopSaleList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	var shopSaleList []int
	err = json.Unmarshal([]byte(configmanager.StaticJsons["shopSaleList"]), &shopSaleList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
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
	response := models.NewInitResponse(hitodamaShopSaleList, shopSaleList, ymoneyShopSaleList, noticePageList)
	resJson, _ := json.Marshal(*response)
	encryptedResJson, _ := nhnrequests.EncryptAndEncode(string(resJson[:]))
	w.Write([]byte(encryptedResJson))
}
