set export := true
set dotenv-load := true

BASENAME := `basename $(pwd)`
CONTAINER_TOOL := `command -v podman >/dev/null && echo podman || echo docker`
CONTAINER_NAME := "${BASENAME}_puniemu_1"
IMAGE_NAME := "${BASENAME}_puniemu"

help:
    just --list
    echo "Using {{CONTAINER_TOOL}}, {{CONTAINER_NAME}} {{IMAGE_NAME}}"

up:
    {{CONTAINER_TOOL}} compose up --build -d

down:
    {{CONTAINER_TOOL}} stop {{CONTAINER_NAME}}

rm:
    just down || true
    {{CONTAINER_TOOL}} rm {{CONTAINER_NAME}} || {{CONTAINER_TOOL}} image rm {{IMAGE_NAME}}

reset:
    just down || true
    just up

exec CMD:
    {{CONTAINER_TOOL}} exec -it {{CONTAINER_NAME}} {{CMD}}

attach:
    {{CONTAINER_TOOL}} container attach {{CONTAINER_NAME}}

log:
    echo "Warning: using ctrl-c will stop the container. Kill the terminal instead."
    {{CONTAINER_TOOL}} container attach {{CONTAINER_NAME}}