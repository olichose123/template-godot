import argparse
import os
import shutil
import json
from pathlib import Path

# Try to import questionary for a better UX, fallback to basic input if not available
try:
    import questionary
    HAS_QUESTIONARY = True
except ImportError:
    HAS_QUESTIONARY = False

# The script is inside template-godot, so its parent is the 'godot' folder.
TEMPLATE_DIR = Path(__file__).parent
DEFAULT_TARGET_PARENT = TEMPLATE_DIR.parent
METADATA_FILE = ".template.json"

def get_input(message, default=""):
    if HAS_QUESTIONARY:
        return questionary.text(message, default=default).ask()
    else:
        val = input(f"{message} [{default}]: ").strip()
        return val if val else default

def confirm(message, default=True):
    if HAS_QUESTIONARY:
        return questionary.confirm(message, default=default).ask()
    else:
        val = input(f"{message} (y/n) [{'y' if default else 'n'}]: ").strip().lower()
        if not val: return default
        return val == 'y'

def select(message, choices):
    if HAS_QUESTIONARY:
        return questionary.select(message, choices=choices).ask()
    else:
        print(message)
        for i, choice in enumerate(choices):
            print(f"{i+1}. {choice}")
        while True:
            try:
                val = int(input("Selection: "))
                if 1 <= val <= len(choices):
                    return choices[val-1]
            except ValueError:
                pass

def apply_template_to_content(content, codename):
    return content.replace("{{CODENAME}}", codename)

def process_file(file_path, codename):
    # Text replacement in content
    try:
        content = file_path.read_text(encoding="utf-8")
        new_content = apply_template_to_content(content, codename)
        if content != new_content:
            file_path.write_text(new_content, encoding="utf-8")
    except UnicodeDecodeError:
        pass # Skip binary files

    # Rename file if it contains {{CODENAME}}
    if "{{CODENAME}}" in file_path.name:
        new_name = file_path.name.replace("{{CODENAME}}", codename)
        new_path = file_path.with_name(new_name)
        file_path.rename(new_path)
        return new_path
    return file_path

def create_project(path_input, author=""):
    # If the user just provides a name, put it in the 'godot' folder (one level up from this script)
    input_path = Path(path_input)
    if len(input_path.parts) == 1:
        target_dir = (DEFAULT_TARGET_PARENT / input_path).resolve()
    else:
        target_dir = input_path.resolve()
        
    codename = target_dir.name

    if target_dir.exists():
        print(f"Error: Directory '{target_dir}' already exists.")
        return

    print(f"Creating project '{codename}' at '{target_dir}'...")
    
    # Copy everything, excluding internal git/godot folders
    shutil.copytree(TEMPLATE_DIR, target_dir, ignore=shutil.ignore_patterns('.git', '.godot', 'bin', 'obj'))

    # Walk through and rename/replace
    for root, dirs, files in os.walk(target_dir, topdown=False):
        # Process files
        for name in files:
            process_file(Path(root) / name, codename)

        # Rename directories containing {{CODENAME}}
        for name in dirs:
            dir_path = Path(root) / name
            if "{{CODENAME}}" in name:
                new_name = name.replace("{{CODENAME}}", codename)
                dir_path.rename(Path(root) / new_name)

    # Update mod.json with author
    mod_json_path = target_dir / "Mods" / "Core" / "mod.json"
    if mod_json_path.exists() and author:
        try:
            mod_data = json.loads(mod_json_path.read_text())
            mod_data["author"] = author
            mod_json_path.write_text(json.dumps(mod_data, indent=4))
        except Exception as e:
            print(f"Warning: Could not update mod.json: {e}")

    # Save metadata for future syncs
    metadata = {
        "codename": codename,
        "template_version": "2.0.0",
        "author": author
    }
    (target_dir / METADATA_FILE).write_text(json.dumps(metadata, indent=4))

    # Remove the generator script and related files from the target project
    files_to_remove = ["create_project.py", "pyproject.toml"]
    for f in files_to_remove:
        path = target_dir / f
        if path.exists():
            path.unlink()

    print(f"\nSuccessfully created project '{codename}'.")
    print(f"Location: {target_dir}")
    print(f"Next steps:")
    print(f"1. Open the project folder in VS Code.")
    print(f"2. Run 'dotnet build' or press F5.")

def sync_project(target_input):
    # If the user just provides a name, check the 'godot' folder
    input_path = Path(target_input)
    if len(input_path.parts) == 1:
        target_dir = (DEFAULT_TARGET_PARENT / input_path).resolve()
    else:
        target_dir = input_path.resolve()

    metadata_path = target_dir / METADATA_FILE
    
    if not metadata_path.exists():
        print(f"Error: '{target_dir}' is not a valid template project (missing {METADATA_FILE}).")
        return

    metadata = json.loads(metadata_path.read_text())
    codename = metadata["codename"]

    print(f"Syncing template updates to project '{codename}' at '{target_dir}'...")

    # Define what to sync
    sync_patterns = [
        "Source/{{CODENAME}}/Core",
        "Source/{{CODENAME}}/Data/AssetManager.cs",
        "Source/{{CODENAME}}/Data/AssetFile.cs",
        "Source/{{CODENAME}}/Data/ResourceReference.cs",
        "Source/{{CODENAME}}/Data/ResourceReferenceJsonConverter.cs",
        ".vscode/tasks.json",
        ".editorconfig",
        ".gitignore"
    ]

    for pattern in sync_patterns:
        template_src = TEMPLATE_DIR / pattern
        target_subpath = pattern.replace("{{CODENAME}}", codename)
        target_dest = target_dir / target_subpath

        if template_src.is_dir():
            for root, _, files in os.walk(template_src):
                rel_root = Path(root).relative_to(TEMPLATE_DIR)
                target_root = target_dir / Path(str(rel_root).replace("{{CODENAME}}", codename))
                target_root.mkdir(parents=True, exist_ok=True)
                
                for name in files:
                    src_file = Path(root) / name
                    dest_file = target_root / name
                    content = src_file.read_text(encoding="utf-8")
                    processed_content = apply_template_to_content(content, codename)
                    dest_file.write_text(processed_content, encoding="utf-8")
                    print(f"  Updated: {dest_file.relative_to(target_dir)}")
        else:
            if template_src.exists():
                target_dest.parent.mkdir(parents=True, exist_ok=True)
                content = template_src.read_text(encoding="utf-8")
                processed_content = apply_template_to_content(content, codename)
                target_dest.write_text(processed_content, encoding="utf-8")
                print(f"  Updated: {target_dest.relative_to(target_dir)}")

    print(f"\nSync complete for '{codename}'.")

def interactive_mode():
    mode = select("What would you like to do?", ["Create New Project", "Sync Existing Project", "Exit"])
    
    if mode == "Create New Project":
        name = get_input("Project Name (will be created in godot/ folder):")
        if not name:
            print("Operation cancelled: Name is required.")
            return
        author = get_input("Author Name:", default="Olib")
        create_project(name, author)
    elif mode == "Sync Existing Project":
        # List projects in the godot folder
        projects = []
        for d in DEFAULT_TARGET_PARENT.iterdir():
            if d.is_dir() and (d / METADATA_FILE).exists():
                projects.append(d.name)
        
        if not projects:
            print("No valid projects found to sync in the godot/ folder.")
            target = get_input("Enter path to project to sync manually:")
            if not target or not Path(target).exists(): return
            sync_project(target)
        else:
            target = select("Select project to sync:", projects)
            if target:
                if confirm(f"This will overwrite framework files in '{target}'. Continue?"):
                    sync_project(target)
    else:
        print("Goodbye!")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Godot Project Template CLI")
    parser.add_argument("path", nargs="?", help="The name or path for the project.")
    parser.add_argument("--author", help="The author name for the project.")
    parser.add_argument("--sync", help="Target project name or path to sync template updates to.")
    parser.add_argument("--interactive", "-i", action="store_true", help="Run in interactive mode.")
    
    args = parser.parse_args()

    if args.interactive or (not args.path and not args.sync):
        interactive_mode()
    elif args.sync:
        sync_project(args.sync)
    else:
        create_project(args.path, args.author or "Olib")
