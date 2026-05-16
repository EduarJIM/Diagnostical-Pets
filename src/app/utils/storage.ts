export interface UserAccount {
  name: string;
  email: string;
  password?: string;
  phone?: string;
}

// System keys that shouldn't be prefixed
const SYSTEM_KEYS = ['users', 'currentUser'];

// Base storage functions that don't intercept keys
const _getRawData = <T,>(key: string, defaultValue: T): T => {
  try {
    const data = localStorage.getItem(key);
    return data ? JSON.parse(data) : defaultValue;
  } catch (error) {
    return defaultValue;
  }
};

const _setRawData = <T,>(key: string, value: T): void => {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch (error) {
    console.error(error);
  }
};

// Auth Functions
export const getCurrentUser = (): UserAccount | null => {
  return _getRawData('currentUser', null);
};

export const setCurrentUser = (user: UserAccount | null) => {
  if (user) {
    _setRawData('currentUser', user);
  } else {
    localStorage.removeItem('currentUser');
  }
};

export const getUsers = (): UserAccount[] => {
  return _getRawData('users', []);
};

export const saveUsers = (users: UserAccount[]) => {
  _setRawData('users', users);
};

// Intercepted storage functions for User-Specific Sandboxing
const getUserScopedKey = (key: string) => {
  if (SYSTEM_KEYS.includes(key)) return key;
  const user = getCurrentUser();
  if (!user) return key; // If not logged in, fallback to raw key
  return `${key}_${user.email}`;
};

export const getStorageData = <T,>(key: string, defaultValue: T): T => {
  const scopedKey = getUserScopedKey(key);
  try {
    const data = localStorage.getItem(scopedKey);
    return data ? JSON.parse(data) : defaultValue;
  } catch (error) {
    console.error(`Error reading ${scopedKey} from localStorage:`, error);
    return defaultValue;
  }
};

export const setStorageData = <T,>(key: string, value: T): void => {
  const scopedKey = getUserScopedKey(key);
  try {
    localStorage.setItem(scopedKey, JSON.stringify(value));
  } catch (error) {
    console.error(`Error writing ${scopedKey} to localStorage:`, error);
  }
};
