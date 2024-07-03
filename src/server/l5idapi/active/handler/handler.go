package handler

import (
	"encoding/json"
	"net/http"

	activeModels "github.com/SuperTavor/Puniemu/src/server/l5idapi/active/models"

	apiModels "github.com/SuperTavor/Puniemu/src/server/l5idapi/models"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	println("l5api: handling active")
	queryParams := r.URL.Query()
	w.Header().Set("Content-Type", "application/json")
	if !queryParams.Has("udkey") {
		res := activeModels.NewActiveGoodResponse(apiModels.NewKeyAutoGen("new udkey", "d-"))
		json, _ := json.Marshal(res)
		w.Write(json)
	} else {
		udkey := queryParams.Get("udkey")
		res := activeModels.NewActiveGoodResponse(udkey)
		json, _ := json.Marshal(res)
		w.Write(json)
	}

}
