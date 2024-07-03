package imgserver

import (
	"fmt"
	"log"
	"net/http"
	"os"
	"strings"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	toDownload := fmt.Sprintf("srv_data/dataDownload/%s", strings.TrimPrefix(r.URL.Path, "/dataDownload/"))

	fileContent, err := os.ReadFile(toDownload)
	if err != nil {
		log.Println(err)
		return
	}
	w.Header().Set("Transfer-Encoding", "identity")
	w.Write(fileContent)
}
