#!/usr/bin/env python3
"""Package saved Unity sources and GGJ materials; does not build or upload."""
from datetime import datetime
import hashlib
import json
from pathlib import Path
import subprocess
import zipfile

ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / 'YiGuangGameJam'
OUTPUT = PROJECT / 'Builds' / 'Submission'
PREFIX = 'YiGuangGameJam'


def collect(directory, destination):
    if not directory.is_dir():
        raise FileNotFoundError(directory)
    for path in sorted(directory.rglob('*')):
        if path.is_symlink():
            raise ValueError(f'Symlink needs manual review: {path}')
        if path.is_file() and path.name != '.DS_Store' and not path.name.startswith('._'):
            yield path, f'{PREFIX}/{destination}/{path.relative_to(directory).as_posix()}'


def main():
    entries = [(ROOT / 'license.txt', f'{PREFIX}/license.txt')]
    for name in ('source', 'release', 'press', 'other'):
        entries.extend(collect(ROOT / name, name))
    for name in ('Assets', 'Packages', 'ProjectSettings'):
        entries.extend(collect(PROJECT / name, f'source/YiGuangGameJam/{name}'))
    names = [name for _, name in entries]
    if len(names) != len(set(names)):
        raise ValueError('Duplicate paths in submission; keep only instructions in repository source/.')
    for required in ('Packages/manifest.json', 'Packages/packages-lock.json',
                     'ProjectSettings/ProjectVersion.txt', 'ProjectSettings/EditorBuildSettings.asset'):
        if not (PROJECT / required).is_file():
            raise FileNotFoundError(PROJECT / required)
    for path, _ in entries:
        if not path.is_file() or path.is_symlink():
            raise ValueError(f'Not a regular file: {path}')
    OUTPUT.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now().strftime('%Y%m%d-%H%M%S-%f')
    output = OUTPUT / f'YiGuangGameJam-submission-DRAFT-{stamp}.zip'
    commit = subprocess.check_output(['git', '-C', str(ROOT), 'rev-parse', 'HEAD'], text=True).strip()
    records = []
    with zipfile.ZipFile(output, 'x', compression=zipfile.ZIP_DEFLATED, compresslevel=6) as archive:
        for path, name in entries:
            before = path.stat()
            data = path.read_bytes()
            after = path.stat()
            if (before.st_size, before.st_mtime_ns) != (after.st_size, after.st_mtime_ns):
                raise RuntimeError(f'File changed while packaging: {path}; save and retry.')
            info = zipfile.ZipInfo.from_file(path, arcname=name)
            archive.writestr(info, data, compress_type=zipfile.ZIP_DEFLATED, compresslevel=6)
            records.append({'path': name, 'bytes': len(data), 'sha256': hashlib.sha256(data).hexdigest()})
        manifest = {'status': 'DRAFT; see other/SUBMISSION_CHECKLIST.md',
                    'generated_at': datetime.now().astimezone().isoformat(),
                    'base_commit': commit,
                    'source': 'Current saved working files, including uncommitted changes',
                    'files': records}
        archive.writestr(f'{PREFIX}/other/PACKAGE_MANIFEST.json',
                         json.dumps(manifest, ensure_ascii=False, indent=2))
    with zipfile.ZipFile(output) as archive:
        bad = archive.testzip()
        if bad:
            raise RuntimeError(f'Archive integrity failure: {bad}')
    print(f'DRAFT ZIP: {output}')
    print(f'Files: {len(records)} + manifest; size: {output.stat().st_size:,} bytes')
    print('Structure and CRC checked. Build, playtesting, licensing and upload remain manual.')


if __name__ == '__main__':
    main()
