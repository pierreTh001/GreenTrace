const KEY = "greentrace-db"

export function loadDb<T>(): T | null {
  try {
    const raw = localStorage.getItem(KEY)
    return raw ? JSON.parse(raw) as T : null
  } catch {
    return null
  }
}

export function saveDb<T>(db: T) {
  localStorage.setItem(KEY, JSON.stringify(db))
}
