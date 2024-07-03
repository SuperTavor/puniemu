package imgserver

import (
	"fmt"
	"log"
	"net/http"
	"os"
	"strings"
)

var cachedFiles map[string][]byte

func Handle(w http.ResponseWriter, r *http.Request) {
	if _, ok := cachedFiles[r.URL.Path]; ok {
		w.Write(cachedFiles[r.URL.Path])
	} else {
		toDownload := fmt.Sprintf("srv_data/dataDownload/%s", strings.TrimPrefix(r.URL.Path, "/dataDownload/"))

		fileContent, err := os.ReadFile(toDownload)
		if err != nil {
			log.Println(err)
			return
		}
		w.Write(fileContent)
		cachedFiles[r.URL.Path] = fileContent
		log.Printf("Cached %s\n", r.URL.Path)
	}

}
