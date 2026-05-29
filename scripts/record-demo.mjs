/**
 * Records an AccessHub UI walkthrough and saves docs/demo/accesshub-demo.gif
 * Requires API (5177) and Web (5173) to be running.
 */
import { mkdir, rm } from 'node:fs/promises';
import { spawn } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';
import ffmpegPath from 'ffmpeg-static';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(__dirname, '..');
const outDir = path.join(root, 'docs', 'demo');
const videoDir = path.join(outDir, '_video');
const webmPath = path.join(videoDir, 'recording.webm');
const gifPath = path.join(outDir, 'accesshub-demo.gif');

const WEB_URL = process.env.WEB_URL ?? 'http://localhost:5173';
const VIEWPORT = { width: 1280, height: 720 };

async function wait(ms) {
  return new Promise((r) => setTimeout(r, ms));
}

async function login(page, email, password) {
  await page.goto(`${WEB_URL}/login`);
  await page.waitForLoadState('networkidle');
  await page.getByLabel('Email').fill(email);
  await page.getByLabel('Password').fill(password);
  await page.getByRole('button', { name: 'Sign In' }).click();
  await page.waitForURL(/\/(?!login)/);
  await wait(800);
}

async function recordDemo() {
  await rm(outDir, { recursive: true, force: true });
  await mkdir(videoDir, { recursive: true });

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: VIEWPORT,
    recordVideo: { dir: videoDir, size: VIEWPORT },
  });
  const page = await context.newPage();

  // --- Admin flow ---
  await login(page, 'admin@acme.local', 'Admin123!');
  await wait(1200);

  await page.getByRole('link', { name: 'Roles' }).click();
  await wait(1500);

  await page.getByRole('link', { name: 'Invoices' }).click();
  await wait(1500);

  await page.getByRole('link', { name: 'Audit Log' }).click();
  await wait(1500);

  await page.getByRole('link', { name: 'Users' }).click();
  await wait(1200);

  await page.getByRole('button', { name: 'Logout' }).click();
  await page.waitForURL(/login/);
  await wait(600);

  // --- Viewer flow (read-only) ---
  await login(page, 'viewer@acme.local', 'Viewer123!');
  await wait(1000);

  await page.getByRole('link', { name: 'Invoices' }).click();
  await wait(2000); // no "New Invoice" button for viewer

  await page.getByRole('link', { name: 'Roles' }).click();
  await wait(1500);

  await wait(800);
  await context.close();
  await browser.close();

  // Playwright names the file arbitrarily — find the .webm
  const { readdir } = await import('node:fs/promises');
  const files = await readdir(videoDir);
  const webm = files.find((f) => f.endsWith('.webm'));
  if (!webm) throw new Error('No webm recording found');
  const recordedPath = path.join(videoDir, webm);

  await convertToGif(recordedPath, gifPath);
  await rm(videoDir, { recursive: true, force: true });
  console.log(`\nDemo GIF saved: ${gifPath}`);
}

function convertToGif(input, output) {
  return new Promise((resolve, reject) => {
    const args = [
      '-y',
      '-i', input,
      '-vf', 'fps=12,scale=960:-1:flags=lanczos,split[s0][s1];[s0]palettegen=max_colors=128[p];[s1][p]paletteuse=dither=bayer',
      '-loop', '0',
      output,
    ];
    const proc = spawn(ffmpegPath, args, { stdio: 'inherit' });
    proc.on('close', (code) => (code === 0 ? resolve() : reject(new Error(`ffmpeg exited ${code}`))));
  });
}

async function waitForServer(url, attempts = 60) {
  for (let i = 0; i < attempts; i++) {
    try {
      const res = await fetch(url);
      if (res.status > 0 && res.status < 500) return;
    } catch {
      /* retry */
    }
    await wait(500);
  }
  throw new Error(`Service not reachable: ${url}`);
}

async function main() {
  console.log('Waiting for API and Web...');
  await waitForServer('http://localhost:5177/api/permissions'); // 401 = API is up
  await waitForServer(WEB_URL);
  console.log('Recording demo...');
  await recordDemo();
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
