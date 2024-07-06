package handler

import (
	"encoding/json"
	"log"
	"net/http"

	apiModels "github.com/SuperTavor/Puniemu/src/server/l5idapi/models"

	createGdKeyModels "github.com/SuperTavor/Puniemu/src/server/l5idapi/create_gdkey/models"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	println("l5api: handling createGdKey")

	query := r.URL.Query()
	w.Header().Set("Content-Type", "application/json")
	if !query.Has("udkey") {
		badResponse := apiModels.NewBadResponse("Unknown UDKey", 4009)
		json, _ := json.Marshal(badResponse)
		w.Write(json)
	} else {
		goodResponse, err := createGdKeyModels.NewCreateGDKeyGoodResponse(query.Get("udkey"))
		if err != nil {
			log.Println(err)
			http.Error(w, "Internal server error", http.StatusInternalServerError)
			return
		}
		json, _ := json.Marshal(goodResponse)
		w.Write(json)
	}
}
