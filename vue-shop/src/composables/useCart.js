import { ref, computed, watch } from 'vue'

const CART_KEY = 'vue-shop-cart'
const PROMO_KEY = 'vue-shop-promo'

// Доступные промокоды: код -> размер скидки (доля от цены)
const PROMO_CODES = {
  WEB: 0.1, // скидка 10%
}

/** Безопасное чтение из localStorage (данные могут быть повреждены). */
function loadJSON(key, fallback) {
  try {
    const raw = localStorage.getItem(key)
    return raw ? JSON.parse(raw) : fallback
  } catch {
    return fallback
  }
}

// Состояние объявлено вне функции — значит корзина общая (синглтон)
// для всех компонентов, которые вызовут useCart().
const items = ref(loadJSON(CART_KEY, []))
const promo = ref(localStorage.getItem(PROMO_KEY) || '')

// Сохраняем корзину при любом изменении — переживает перезагрузку страницы.
watch(
  items,
  (value) => localStorage.setItem(CART_KEY, JSON.stringify(value)),
  { deep: true },
)
watch(promo, (value) => {
  if (value) localStorage.setItem(PROMO_KEY, value)
  else localStorage.removeItem(PROMO_KEY)
})

export function useCart() {
  // Размер скидки по применённому промокоду (0, если промокода нет)
  const discount = computed(() => PROMO_CODES[promo.value] ?? 0)

  // Сумма без скидки
  const totalBefore = computed(() =>
    items.value.reduce((sum, item) => sum + item.price * item.qty, 0),
  )

  // Итоговая сумма с учётом скидки
  const total = computed(() => Math.round(totalBefore.value * (1 - discount.value)))

  // Сколько денег сэкономлено
  const saved = computed(() => totalBefore.value - total.value)

  // Общее количество единиц товара (для счётчика в шапке)
  const count = computed(() => items.value.reduce((sum, item) => sum + item.qty, 0))

  function addToCart(product) {
    const found = items.value.find((item) => item.id === product.id)
    if (found) {
      found.qty++
    } else {
      items.value.push({ ...product, qty: 1 })
    }
  }

  function removeFromCart(id) {
    items.value = items.value.filter((item) => item.id !== id)
  }

  function increase(id) {
    const found = items.value.find((item) => item.id === id)
    if (found) found.qty++
  }

  function decrease(id) {
    const found = items.value.find((item) => item.id === id)
    if (!found) return
    if (found.qty > 1) found.qty--
    else removeFromCart(id)
  }

  function clearCart() {
    items.value = []
  }

  function inCart(id) {
    return items.value.some((item) => item.id === id)
  }

  /** Применить промокод. Возвращает true, если код существует. */
  function applyPromo(code) {
    const normalized = String(code).trim().toUpperCase()
    if (PROMO_CODES[normalized]) {
      promo.value = normalized
      return true
    }
    return false
  }

  function resetPromo() {
    promo.value = ''
  }

  /** Цена товара с учётом скидки (нужна для отображения -10% на карточке). */
  function priceWithDiscount(price) {
    return Math.round(price * (1 - discount.value))
  }

  return {
    items,
    promo,
    discount,
    totalBefore,
    total,
    saved,
    count,
    addToCart,
    removeFromCart,
    increase,
    decrease,
    clearCart,
    inCart,
    applyPromo,
    resetPromo,
    priceWithDiscount,
  }
}

/** Формат цены: 89990 -> "89 990 ₽" */
export function formatPrice(value) {
  return new Intl.NumberFormat('ru-RU').format(value) + ' ₽'
}
