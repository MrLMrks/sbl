@echo off
taskkill /F /IM chromedriver.exe
taskkill /F /IM chrome.exe
start "" "C:\Program Files\Google\Chrome\Application\chrome.exe" --remote-debugging-port=9222 --new-window "https://e.sb.lt" --user-data-dir="C:\ChromeDebug"
