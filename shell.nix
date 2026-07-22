# Loads a shell environment with podman loaded
let
  nixpkgs = fetchTarball "https://github.com/NixOS/nixpkgs/tarball/nixos-24.05";
  pkgs = import nixpkgs { config = {}; overlays = []; };
in
pkgs.mkShellNoCC {
  packages = with pkgs; [
    just
    lolcat
    jq
    bats
    inotify-tools
    podman
    podman-compose
    # podman dependencies
    fuse-overlayfs
  ];

  # On shell startup
  shellHook = ''
    # Load environment variables from .env file
    set -a
    source .env
    set +a

    # Setup Podman config directory
    export PODMAN_CONFIG_DIR="$PWD/.config/podman"
    export CONTAINERS_CONF="$PODMAN_CONFIG_DIR/containers.conf"
    export CONTAINERS_STORAGE_CONF="$PODMAN_CONFIG_DIR/storage.conf"
    # Add symlinks to the project's config (podman has no env variable for them)
    echo "Running checks for podman config..." | lolcat
    if [ ! -d "$PODMAN_CONFIG_DIR" ]; then
      echo "Creating podman config directory at $PODMAN_CONFIG_DIR"
      sudo mkdir --verbose -p "$PODMAN_CONFIG_DIR";
    fi
    if [ ! -f ~/.config/containers/policy.json ]; then
      echo "Linking podman policy.json configuration at ~/.config/containers/policy.json"
      sudo ln --verbose "$PODMAN_CONFIG_DIR/policy.json" ~/.config/containers/policy.json
    else
      echo "Warning: a podman config file already exists on the system."
      echo "~/.config/containers/policy.json already exists and I'm not overwriting it."
      echo "|-> Run 'sudo ln "$PODMAN_CONFIG_DIR/policy.json" ~/.config/containers/policy.json' yourself"
      echo ""
    fi
    if [ ! -f ~/.config/containers/registries.conf ]; then
      echo "Linking podman registries.conf configuration at ~/.config/containers/registries.conf"
      sudo ln --verbose "$PODMAN_CONFIG_DIR/registries.conf" ~/.config/containers/registries.conf
    else
      echo "Warning: a podman config file already exists on the system."
      echo "~/.config/containers/registries.conf already exists and I'm not overwriting it."
      echo "|-> Run 'sudo ln "$PODMAN_CONFIG_DIR/registries.conf" ~/.config/containers/registries.conf' yourself"
      echo ""
    fi

    echo -e "
      [storage]
      driver = \"overlay\"
      runroot = \"$PWD/.containers/run\"
      graphroot = \"$PWD/.containers/storage\"
      [storage.options]
      mount_program = \"$(which fuse-overlayfs)\"
    " > $CONTAINERS_STORAGE_CONF
    echo "Done setting up podman config..." | lolcat

    echo "Welcome to Globuland's wib wob reloaded !" | lolcat
  '';
}