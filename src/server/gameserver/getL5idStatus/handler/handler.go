package handler

import (
	"net/http"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	//simply return code 200, as this response doesn't need to by anything valid to be accepted by the game.
	w.WriteHeader(http.StatusOK)
}
