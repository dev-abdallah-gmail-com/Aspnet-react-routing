#!/usr/bin/env bash
# سكربت تهيئة البيئة: يثبّت .NET SDK ويجهّز الحزم
set -euo pipefail

# تثبيت .NET SDK 8.0 إن لم يكن موجودًا
if ! command -v dotnet >/dev/null 2>&1 && [ ! -x /usr/share/dotnet/dotnet ]; then
  echo "تثبيت .NET SDK 8.0…"
  apt-get update -y
  apt-get install -y dotnet-sdk-8.0
fi
export PATH="$PATH:/usr/share/dotnet"

echo "استعادة حزم الخادم…"
dotnet restore Server/Server.csproj

echo "تثبيت حزم الواجهة…"
(cd client && npm install)

echo "اكتملت التهيئة. للتشغيل: (cd Server && dotnet run) و (cd client && npm run dev)"
