package punitableparser

import (
	"strings"
)

type Table struct {
	Rows []Row
}

type Row struct {
	Items []string
}

func removeItem(slice []string, index int) []string {
	return append(slice[:index], slice[index+1:]...)
}
func insertItemAtIndex(slice []string, item string, insertIndex int) []string {
	newSlice := make([]string, 0, len(slice)+1)
	newSlice = append(newSlice, slice[:insertIndex]...)
	newSlice = append(newSlice, item)
	newSlice = append(newSlice, slice[insertIndex:]...)
	return newSlice
}
func (tbl *Table) AsMap(keyIndex int) map[string][]string {
	outputMap := make(map[string][]string)

	for _, row := range tbl.Rows {
		key := row.Items[keyIndex]
		outputMap[key] = removeItem(row.Items, keyIndex)
	}

	return outputMap
}
func (tbl *Table) ApplyMap(tableMap map[string][]string, keyIndex int) {
	var index int = 0
	for k, v := range tableMap {
		items := insertItemAtIndex(v, k, keyIndex)
		tbl.Rows[index].Items = items
		index++
	}
}
func (tbl *Table) Get(rowIndex int, stringIndex int) string {
	return tbl.Rows[rowIndex].Items[stringIndex]
}

func NewTable(tableStr string) *Table {
	initialItems := strings.Split(tableStr, "|")
	rows := make([]Row, 1)
	for _, item := range initialItems {
		currentRow := &rows[len(rows)-1]
		if !strings.Contains(item, "*") {
			currentRow.Items = append(currentRow.Items, item)
		} else {
			parts := strings.Split(item, "*")
			currentRow.Items = append(currentRow.Items, parts[0])
			rows = append(rows, Row{
				Items: []string{parts[1]},
			})
		}
	}
	return &Table{Rows: rows}
}
