// Lightweight fetch wrapper with bearer token from AuthService
import { AuthService } from './AuthService'

const API_BASE = '' // same origin; adjust if needed

async function request<T>(method: string, url: string, body?: any): Promise<T> {
  const token = AuthService.token()
  const res = await fetch(API_BASE + url, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    body: body ? JSON.stringify(body) : undefined
  })
  if (!res.ok) {
    const txt = await res.text().catch(() => '')
    throw new Error(`${res.status} ${res.statusText}: ${txt}`)
  }
  // Some endpoints return empty body
  const contentType = res.headers.get('content-type') || ''
  if (!contentType.includes('application/json')) return undefined as unknown as T
  return await res.json() as T
}

export const ApiClient = {
  get: <T>(url: string) => request<T>('GET', url),
  post: <T>(url: string, body?: any) => request<T>('POST', url, body),
  put: <T>(url: string, body?: any) => request<T>('PUT', url, body),
  del: <T>(url: string) => request<T>('DELETE', url)
}

