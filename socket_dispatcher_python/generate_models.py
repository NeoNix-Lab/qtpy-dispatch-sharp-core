import os
import sys
import subprocess
from pathlib import Path


def generate_models(schema_dir: Path, out_dir: Path) -> None:
    out_dir.mkdir(parents=True, exist_ok=True)
    imports = []
    for schema_file in sorted(schema_dir.glob('*.json')):
        model_name = schema_file.stem
        output_file = out_dir / f"{model_name}.py"
        subprocess.run([
            sys.executable,
            '-m', 'datamodel_code_generator',
            '--input', str(schema_file),
            '--input-file-type', 'jsonschema',
            '--output', str(output_file),
            '--output-model-type', 'dataclasses.dataclass',
        ], check=True)
        imports.append(f"from .{model_name} import *")
    if imports:
        with open(out_dir / '__init__.py', 'w') as f:
            f.write('\n'.join(imports) + '\n')


def main() -> None:
    schema_path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path(os.environ.get('QT_SDK_PATH', '.'))
    out_dir = Path(__file__).resolve().parent / 'models'
    generate_models(schema_path, out_dir)


if __name__ == '__main__':
    main()
