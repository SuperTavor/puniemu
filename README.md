# Puniemu

A WIP server emulator for Yo-kai Watch Puni Puni.

<p align="center">
<img src=https://i.imgur.com/zO49hMu.png alt=logo width=320>
</p>

## What is this? 👀

Essentially, this is a private server for *Yo-kai Watch Puni Puni* written in C#. It can be hosted by anyone, anywhere as a drop-in replacement for NHN's servers. This allows everyone to play even after the servers shut down. Additionally, you can have full control over your experience by modifying the code.

## DISCLAIMERS 🚫

Please note that this project is completely non-profit. As such, in-app purchases are completely disabled.

The project is in no way affiliated with NHN. We do not intend to infringe on their copyright in any way, shape or form.

## Credits 🎬

- Zura - Main dev
- DarkCraft - Main dev
- wibwob_yt - Dev
- onepiecefreak3 - Reverse engineering help
- kuronosuFear - Reverse engineering help
- picky_x_keizen - Logo

------------------
## Getting started

-------------
#### Using nix (optional)

This repo contains a `shell.nix`, nix is a powerful tool that creates a virtual envrionment with the neccesary tooling loaded.  
The project's dependencies will be installed automatically, but only for this project, and will not results in conflicts with other projects' dependencies. 
Read the doc [here](https://install.determinate.systems/).   
Install with these commands:    
``` bash
curl -L https://install.determinate.systems/nix | sh -s -- install --no-confirm
. /nix/var/nix/profiles/default/etc/profile.d/nix-daemon.sh
nix-shell --run "nix --version"
echo "This is using nix tools" | lolcat
```
Open a new terminal to load nix.    
Finally, run `nix-shell` to enter the dev environment.   

----------

#### Using just (recommended)

[Just](https://github.com/casey/just) is a useful command binding tools.    
Run `just` and see the list of binding available.   
``` bash
$> just
just --list
Available recipes:
    attach
    down
    exec CMD
    help
    log
    reset
    rm
    up
```

----------

1. Copy .env.example into .env and edit the configuration
2. Run the server (from best to worst option):
    - (`nix-shell`) `just up`, let just (and nix) do the heavy lifting 
    - By using the docker-compose.yml to build the puniemu server and postgresql database at the same time
    - Manually using `dotnet run` and postgres

