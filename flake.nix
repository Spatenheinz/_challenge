{
  description = "C# / .NET development shell";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};

        dotnetPkg = pkgs.dotnet-sdk; # v.8

      in {
        devShells.default = pkgs.mkShell {
          name = "dotnet-dev";

          packages = [
            dotnetPkg
            pkgs.roslyn-ls
          ];

          # Prevent .NET from trying to write to the Nix store
          DOTNET_ROOT = "${dotnetPkg}";
          # Silence the .NET telemetry nag
          DOTNET_CLI_TELEMETRY_OPTOUT = "1";
          # Suppress "You may be missing a runtime" warnings on non-NixOS
          DOTNET_NOLOGO = "1";
        };
      }
    );
}
