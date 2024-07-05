package handler

import (
	"net/http"
	"encoding/json"
	"strings"
	"io/ioutil"
	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	// Get request body

	body, err := ioutil.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Unable to read request body", http.StatusBadRequest)
		return
	}
	// Decode and decrypt request body and load it as JSON

        var bodyJson map[string]interface{}
	var tables []string
	bodyJsonString,err := nhnrequests.DecodeAndDecrypt(string(body))
	err = json.Unmarshal([]byte(bodyJsonString), &bodyJson)
	if err != nil {
		println("error")
		println(err)
		return
	}
	// Load base MasterData JSON

	var MasterDataJson map[string]interface{}
	err = json.Unmarshal([]byte(configmanager.StaticJsons["MasterData"]), &MasterDataJson)
	if err != nil {
		println(err)
		return
	}
	// Get table inside requests or the table in all.json (if the request table was set to "all")
	
	if bodyJson["tableNames"].(string) != "all" {
		tables = strings.Split(bodyJson["tableNames"].(string), "|")
	} else {
		var allJson map[string]interface{}
		err = json.Unmarshal([]byte(configmanager.StaticJsons["all"]), &allJson)
		if err != nil {
			println(err)
			return
		}
		tables = strings.Split(allJson["MasterData"].(string), "|")
	}

	// Put every requested table inside the base MasterData JSON
	for _, value := range tables {
		var tempJson map[string]interface{}
		tempStr := configmanager.StaticJsons[value]
		if tempStr != "" {
			json.Unmarshal([]byte(tempStr), &tempJson)
			MasterDataJson[value]=tempJson
		}
	}

	// Encrypt and encode the final base MasterData JSON and send it as a response
	jsonBytes, err := json.Marshal(MasterDataJson)
	if err != nil {
		println(err)
		return
	}
	MasterDataJsonString := string(jsonBytes)
	w.Header().Set("Content-Type", "application/json")
	encodedMasterData, err := nhnrequests.EncryptAndEncode(MasterDataJsonString)
	if err != nil {
		println(err)
		return
	}
	w.Write([]byte(encodedMasterData))
}
