package imgserver

import (
	"fmt"
	"log"
	"net/http"
	"os"
	"strings"
	"sync"
)

var (
	cachedFiles = make(map[string][]byte)
	cacheMutex  sync.RWMutex
	baseDir     = "srv_data/dataDownload/"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	cacheMutex.RLock()
	if fileContent, ok := cachedFiles[r.URL.Path]; ok {
		log.Printf("Already cached: %s\n", r.URL.Path)
		w.Write(fileContent)
		cacheMutex.RUnlock()
		return
	}
	cacheMutex.RUnlock()
	toDownload := fmt.Sprintf("srv_data/dataDownload/%s", strings.TrimPrefix(r.URL.Path, "/dataDownload/"))
	//Check to see if request wants non data download files from the server, which is bad
	if !strings.HasPrefix(toDownload, baseDir) {
		http.Error(w, "Accessing files that don't belong to you sucks.", http.StatusForbidden)
		return
	}
	fileContent, err := os.ReadFile(toDownload)
	if err != nil {
		//Sometimes the game sends a request for .og even though it's supposed to get the m4. Idk why this happens but simply replacing it seems to work
		if strings.HasSuffix(toDownload, ".og") {
			r.URL.Path = fmt.Sprintf("%s.m4", strings.TrimSuffix(r.URL.Path, ".og"))
			Handle(w, r)
			return
		}
		log.Printf("Failed to read file %s: %v\n", toDownload, err)
		http.Error(w, "File not found", http.StatusNotFound)
		return
	}

	cacheMutex.Lock()
	defer cacheMutex.Unlock()
	cachedFiles[r.URL.Path] = fileContent

	log.Printf("Cached %s\n", r.URL.Path)
	w.Write(fileContent)
}
