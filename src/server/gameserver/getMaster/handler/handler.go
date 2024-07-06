package handler

import (
	"encoding/json"
	"io"
	"log"
	"net/http"
	"strings"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
)

// To not unmarshal every time and improve performance, we store the pointers of previously unmarshalled jsons
var unmarshalCache = make(map[string](*map[string]interface{}))

func UnmarshalOrGetFromCache(jsonName, jsonString string) (*map[string]interface{}, error) {
	if _, ok := unmarshalCache[jsonName]; !ok {
		var unmarshalledJson map[string]interface{}
		err := json.Unmarshal([]byte(jsonString), &unmarshalledJson)
		if err != nil {
			log.Println(err)
			return nil, err
		}
		unmarshalCache[jsonName] = &unmarshalledJson
	}
	return unmarshalCache[jsonName], nil
}
func Handle(w http.ResponseWriter, r *http.Request) {
	// Get request body
	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Unable to read request body", http.StatusBadRequest)
		return
	}
	// Decode and decrypt request body and load it as JSON
	var bodyJson map[string]interface{}
	var tables []string
	bodyJsonString, err := nhnrequests.DecodeAndDecrypt(string(body))
	if err != nil {
		log.Println(err)
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	err = json.Unmarshal([]byte(bodyJsonString), &bodyJson)
	if err != nil {
		log.Println(err)
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	// Load base MasterData JSON. the base MasterData JSON contains data other than the requested tables that is shipped with the requested tables.
	var pMasterDataJson *map[string]interface{}
	pMasterDataJson, err = UnmarshalOrGetFromCache("BaseMasterData", configmanager.StaticJsons["MasterData"])
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	MasterDataJson := *pMasterDataJson

	// Get table inside requests or the table in all.json (if the request table was set to "all"). all.json contains all the tables the game would theoretically want if it requests `all`.
	if bodyJson["tableNames"].(string) != "all" {
		tables = strings.Split(bodyJson["tableNames"].(string), "|")
	} else {
		pAllJson, err := UnmarshalOrGetFromCache("all", configmanager.StaticJsons["all"])
		if err != nil {
			log.Println(err)
			return
		}
		allJson := *pAllJson
		tables = strings.Split(allJson["MasterData"].(string), "|")
	}

	// Put every requested table inside the base MasterData JSON
	for _, tableName := range tables {
		if selectedMstJsonContent, ok := configmanager.StaticJsons[tableName]; ok {
			pSelectedMstJsonUnmarshalled, err := UnmarshalOrGetFromCache(tableName, selectedMstJsonContent)
			if err != nil {
				log.Println(err)
				http.Error(w, "Internal server error", http.StatusInternalServerError)
				return
			}
			selectedMstJsonUnmarshalled := *pSelectedMstJsonUnmarshalled
			MasterDataJson[tableName] = selectedMstJsonUnmarshalled
		}
	}

	// Encrypt and encode the edited base MasterData JSON with all the requested tables and send it as a response
	jsonBytes, err := json.Marshal(MasterDataJson)
	if err != nil {
		log.Println(err)
		return
	}
	MasterDataJsonString := string(jsonBytes)
	w.Header().Set("Content-Type", "application/json")
	encodedMasterData, err := nhnrequests.EncryptAndEncode(MasterDataJsonString)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	w.Write([]byte(encodedMasterData))
}
