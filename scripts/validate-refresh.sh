#!/usr/bin/env bash
set -euo pipefail

# Usage:
#   EMAIL=user@example.com PASSWORD=secret [API=https://localhost:7165/api] ./scripts/validate-refresh.sh

API="${API:-https://localhost:7165/api}"
EMAIL="${EMAIL:-}"
PASSWORD="${PASSWORD:-}"
INSECURE="${INSECURE:-true}"

if [[ -z "$EMAIL" || -z "$PASSWORD" ]]; then
  echo "Usage: EMAIL=... PASSWORD=... [API=...] ./scripts/validate-refresh.sh"
  exit 1
fi

CURL_OPTS=()
if [[ "$INSECURE" == "true" ]]; then
  CURL_OPTS+=( -k )
fi

JAR="$(mktemp)"
cleanup(){ rm -f "$JAR"; }
trap cleanup EXIT

echo "1) Login"
curl "${CURL_OPTS[@]}" -sS -c "$JAR" -b "$JAR" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}" \
  "$API/auth/login" >/dev/null

xsrf=$(awk '$0 ~ /XSRF-TOKEN/ {print $7}' "$JAR" | tail -n 1)
rt1=$(awk '$0 ~ /refresh_token/ {print $7}' "$JAR" | tail -n 1)
at1=$(awk '$0 ~ /access_token/ {print $7}' "$JAR" | tail -n 1)

echo "2) /auth/me"
code_me1=$(curl "${CURL_OPTS[@]}" -s -o /dev/null -w "%{http_code}" -c "$JAR" -b "$JAR" "$API/auth/me" || true)

echo "3) /auth/refresh"
code_refresh=$(curl "${CURL_OPTS[@]}" -s -o /dev/null -w "%{http_code}" -c "$JAR" -b "$JAR" \
  -H "X-XSRF-TOKEN: $xsrf" -X POST "$API/auth/refresh" || true)

rt2=$(awk '$0 ~ /refresh_token/ {print $7}' "$JAR" | tail -n 1)
at2=$(awk '$0 ~ /access_token/ {print $7}' "$JAR" | tail -n 1)

echo "4) /auth/me (after)"
code_me2=$(curl "${CURL_OPTS[@]}" -s -o /dev/null -w "%{http_code}" -c "$JAR" -b "$JAR" "$API/auth/me" || true)

rotated="false"
if [[ "$rt1" != "$rt2" && -n "$rt2" ]]; then rotated="true"; fi

echo "Login:        200"
echo "Me (before):  $code_me1"
echo "Refresh:      $code_refresh"
echo "Me (after):   $code_me2"
echo "Rotated RT?:  $rotated"

