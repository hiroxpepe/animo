// validate_examples.mjs
// Phase 2 Task 2-1 — exit gate: validate 3 sample JSON files against animo.schema.json
//
// Usage:
//   node validate_examples.mjs
//
// Exit code 0 = all green. Non-zero = at least one example failed.

import { readFileSync, readdirSync } from 'node:fs';
import { resolve, dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import Ajv from '/home/claude/.npm-global/lib/node_modules/ajv/dist/ajv.js';

const __dirname = dirname(fileURLToPath(import.meta.url));
const SCHEMA_PATH = resolve(__dirname, 'Schemas/animo.schema.json');
const EXAMPLES_DIR = resolve(__dirname, 'examples');

console.log('═══════════════════════════════════════════════════════════════');
console.log(' Animo — Phase_2_1_1 schema validation');
console.log('═══════════════════════════════════════════════════════════════');
console.log(` Schema:   ${SCHEMA_PATH}`);
console.log(` Examples: ${EXAMPLES_DIR}`);
console.log('───────────────────────────────────────────────────────────────');

// Load and compile schema
const schema = JSON.parse(readFileSync(SCHEMA_PATH, 'utf8'));
const ajv = new Ajv.default({
  strict: false,         // allow $id at root, no $defs in Draft-07
  allErrors: true,       // report every problem, not just the first
  verbose: true
});

let validate;
try {
  validate = ajv.compile(schema);
  console.log(' ✅ Schema compiled (Draft-07)');
  console.log(`    definitions: ${Object.keys(schema.definitions).length}`);
} catch (err) {
  console.error(' ❌ Schema failed to compile:');
  console.error(`    ${err.message}`);
  process.exit(2);
}
console.log('───────────────────────────────────────────────────────────────');

// Discover example JSON files
const files = readdirSync(EXAMPLES_DIR)
  .filter(f => f.endsWith('.json'))
  .sort();

if (files.length === 0) {
  console.error(' ❌ No JSON files found in examples/');
  process.exit(3);
}

let pass = 0;
let fail = 0;

for (const f of files) {
  const fullPath = join(EXAMPLES_DIR, f);
  let data;
  try {
    data = JSON.parse(readFileSync(fullPath, 'utf8'));
  } catch (e) {
    console.log(` ❌ ${f}  — JSON parse error: ${e.message}`);
    fail++;
    continue;
  }

  const ok = validate(data);
  if (ok) {
    const personaCount = data.personas?.length ?? 0;
    const kindCount = data.kinds?.length ?? 0;
    console.log(` ✅ ${f.padEnd(26)}  schema_version=${data.schema_version}  kinds=${kindCount}  personas=${personaCount}`);
    pass++;
  } else {
    console.log(` ❌ ${f}`);
    for (const err of validate.errors) {
      console.log(`    ↳ ${err.instancePath || '(root)'}  ${err.message}`);
      if (err.params && Object.keys(err.params).length > 0) {
        console.log(`       params: ${JSON.stringify(err.params)}`);
      }
    }
    fail++;
  }
}

console.log('───────────────────────────────────────────────────────────────');
console.log(` Result: ${pass} passed / ${fail} failed (of ${files.length} files)`);
console.log('═══════════════════════════════════════════════════════════════');

process.exit(fail === 0 ? 0 : 1);
