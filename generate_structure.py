#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Project Structure Generator for AI Migration - WPF Project
Generates a beautiful, readable directory tree structure
Usage: Just double-click this file or run: python generate_structure.py
"""

import os
from pathlib import Path

# ============================================================================
# CONFIGURATION - Edit these to match your project
# ============================================================================

PROJECT_NAME = "CabinetDoc Pro (Project B - C# WPF Desktop App)"
PROJECT_ROOT = Path(__file__).parent  # Current directory

# Folders/files to IGNORE (won't appear in the structure)
IGNORE_PATTERNS = {
    # Directories to skip completely
    'node_modules',
    '.venv',
    'venv',
    'dist',
    'build',
    '.git',
    '.vscode',
    '.vs',
    '__pycache__',
    '.pytest_cache',
    'bin',
    'obj',
    'packages',
    
    # File patterns to skip
    '.pyc',
    '.pyo',
    '.pyd',
    '.log',
    '.sqlite',
    '.db',
    '.user',
    '.suo',
    '.cache',
    '.DS_Store',
    'Thumbs.db',
}

# File extensions to include (empty = include all)
# For WPF: focus on XAML, C#, and config files
INCLUDE_EXTENSIONS = set()  # Empty = include everything (except ignored)

# Maximum depth (None = unlimited, 10 = reasonable default)
MAX_DEPTH = 10

# ============================================================================
# GENERATOR CODE - No need to edit below
# ============================================================================

def should_ignore(name, path):
    """Check if file/folder should be ignored"""
    # Check exact matches
    if name in IGNORE_PATTERNS:
        return True
    
    # Check patterns (contains)
    for pattern in IGNORE_PATTERNS:
        if pattern in name:
            return True
    
    # Check file extensions if filter is set
    if INCLUDE_EXTENSIONS and path.is_file():
        if path.suffix.lower() not in INCLUDE_EXTENSIONS:
            return True
    
    return False


def get_icon(path):
    """Get appropriate emoji icon for file/folder"""
    if path.is_dir():
        return "📁"
    
    ext = path.suffix.lower()
    
    # Programming files
    if ext in ['.js', '.jsx', '.ts', '.tsx']:
        return "⚛️"
    elif ext in ['.py']:
        return "🐍"
    elif ext in ['.cs']:
        return "🔷"
    elif ext in ['.java']:
        return "☕"
    
    # Markup/Style
    elif ext in ['.html', '.htm']:
        return "🌐"
    elif ext in ['.css', '.scss', '.sass', '.less']:
        return "🎨"
    elif ext in ['.xml', '.xaml']:
        return "📋"
    
    # Config/Data
    elif ext in ['.json', '.yaml', '.yml', '.toml']:
        return "⚙️"
    elif ext in ['.csproj', '.sln']:
        return "🔧"
    elif ext in ['.config']:
        return "⚙️"
    elif ext in ['.env']:
        return "🔐"
    elif ext in ['.md', '.txt']:
        return "📝"
    
    # Images
    elif ext in ['.png', '.jpg', '.jpeg', '.gif', '.svg', '.ico']:
        return "🖼️"
    
    # Others
    else:
        return "📄"


def generate_tree(directory, prefix="", depth=0):
    """Recursively generate tree structure"""
    if MAX_DEPTH and depth >= MAX_DEPTH:
        return []
    
    lines = []
    
    try:
        # Get all items and sort (folders first, then files)
        items = sorted(directory.iterdir(), key=lambda x: (x.is_file(), x.name.lower()))
        
        # Filter ignored items
        items = [item for item in items if not should_ignore(item.name, item)]
        
        for i, item in enumerate(items):
            is_last = (i == len(items) - 1)
            
            # Tree characters
            connector = "└── " if is_last else "├── "
            extension = "    " if is_last else "│   "
            
            # Get icon and name
            icon = get_icon(item)
            name = item.name
            
            # Add line
            lines.append(f"{prefix}{connector}{icon} {name}")
            
            # Recurse into directories
            if item.is_dir():
                lines.extend(generate_tree(item, prefix + extension, depth + 1))
    
    except PermissionError:
        pass  # Skip folders we can't access
    
    return lines


def count_items(directory, depth=0):
    """Count files and folders"""
    if MAX_DEPTH and depth >= MAX_DEPTH:
        return 0, 0
    
    files = 0
    folders = 0
    
    try:
        items = list(directory.iterdir())
        items = [item for item in items if not should_ignore(item.name, item)]
        
        for item in items:
            if item.is_dir():
                folders += 1
                f, d = count_items(item, depth + 1)
                files += f
                folders += d
            else:
                files += 1
    except PermissionError:
        pass
    
    return files, folders


def main():
    """Main function"""
    print("=" * 80)
    print(f"🏗️  PROJECT STRUCTURE GENERATOR")
    print("=" * 80)
    print()
    
    # Generate structure
    print(f"📂 Scanning: {PROJECT_ROOT}")
    print(f"🔍 Generating structure...")
    print()
    
    # Count items
    file_count, folder_count = count_items(PROJECT_ROOT)
    
    # Generate tree
    tree_lines = generate_tree(PROJECT_ROOT)
    
    # Build output
    output_lines = [
        "=" * 80,
        f"📦 {PROJECT_NAME}",
        "=" * 80,
        "",
        f"📊 Summary: {folder_count} folders, {file_count} files",
        "",
        "```",
        f"{PROJECT_ROOT.name}/",
        "│"
    ]
    
    output_lines.extend(tree_lines)
    output_lines.append("```")
    output_lines.append("")
    output_lines.append("=" * 80)
    output_lines.append("✅ Structure generated successfully!")
    output_lines.append("📋 Copy the above structure and paste it to the AI")
    output_lines.append("=" * 80)
    
    # Print to console
    output_text = "\n".join(output_lines)
    print(output_text)
    
    # Save to file
    output_file = PROJECT_ROOT / "PROJECT_STRUCTURE.txt"
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(output_text)
    
    print()
    print(f"💾 Also saved to: {output_file}")
    print()
    input("Press ENTER to exit...")


if __name__ == "__main__":
    main()
