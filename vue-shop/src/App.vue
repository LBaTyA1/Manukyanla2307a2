<script setup>
import { ref, computed, watch } from 'vue'
import { products, formatPrice } from './data/products.js'
import ProductCard from './components/ProductCard.vue'
import CartItem from './components/CartItem.vue'

// ---------- Ключи для localStorage ----------
const CART_KEY = 'vue-shop-cart'
const PROMO_KEY = 'vue-shop-promo'

// Доступные промокоды: код -> размер скидки
const PROMO_CODES = {
  WEB: 0.1, // скидка 10%
}

// ---------- Состояние ----------
// Корзина восстанавливается из localStorage при загрузке страницы
const cart = ref(loadCart())
const promo = ref(localStorage.getItem(PROMO_KEY) || '')
const promoInput = ref(promo.value)
const promoError = ref('')

const search = ref('')
const category = ref('all')
const sort = ref('default')

function loadCart() {
  try {
    return JSON.parse(localStorage.getItem(CART_KEY)) || []
  } catch {
    return []
  }
}

// Любое изменение корзины сразу сохраняем — переживает обновление страницы
watch(cart, (value) => localStorage.setItem(CART_KEY, JSON.stringify(value)), { deep: true })
watch(promo, (value) => {
  if (value) localStorage.setItem(PROMO_KEY, value)
  else localStorage.removeItem(PROMO_KEY)
})

// ---------- Каталог: поиск, фильтр, сортировка ----------
const categories = computed(() => [...new Set(products.map((p) => p.category))])

const visibleProducts = computed(() => {
  const query = search.value.trim().toLowerCase()

  let result = products.filter((product) => {
    const matchesSearch =
      !query ||
      product.name.toLowerCase().includes(query) ||
      product.category.toLowerCase().includes(query)
    const matchesCategory = category.value === 'all' || product.category === category.value
    return matchesSearch && matchesCategory
  })

  if (sort.value === 'asc') result = [...result].sort((a, b) => a.price - b.price)
  if (sort.value === 'desc') result = [...result].sort((a, b) => b.price - a.price)

  return result
})

// ---------- Корзина ----------
const discount = computed(() => PROMO_CODES[promo.value] || 0)

const count = computed(() => cart.value.reduce((sum, item) => sum + item.qty, 0))

// Сумма без скидки
const totalBefore = computed(() =>
  cart.value.reduce((sum, item) => sum + item.price * item.qty, 0),
)

// Итоговая сумма: цена каждого товара пересчитана со скидкой
const total = computed(() =>
  cart.value.reduce(
    (sum, item) => sum + Math.round(item.price * (1 - discount.value)) * item.qty,
    0,
  ),
)

const saved = computed(() => totalBefore.value - total.value)

function addToCart(product) {
  const found = cart.value.find((item) => item.id === product.id)
  if (found) found.qty++
  else cart.value.push({ ...product, qty: 1 })
}

function removeFromCart(id) {
  cart.value = cart.value.filter((item) => item.id !== id)
}

function increase(id) {
  const found = cart.value.find((item) => item.id === id)
  if (found) found.qty++
}

function decrease(id) {
  const found = cart.value.find((item) => item.id === id)
  if (!found) return
  if (found.qty > 1) found.qty--
  else removeFromCart(id)
}

function clearCart() {
  cart.value = []
}

function inCart(id) {
  return cart.value.some((item) => item.id === id)
}

// ---------- Промокод ----------
function applyPromo() {
  const code = promoInput.value.trim().toUpperCase()
  if (!code) {
    promoError.value = 'Введите промокод'
    return
  }
  if (!PROMO_CODES[code]) {
    promoError.value = 'Такого промокода не существует'
    return
  }
  promo.value = code
  promoError.value = ''
}

function resetPromo() {
  promo.value = ''
  promoInput.value = ''
  promoError.value = ''
}
</script>

<template>
  <header class="header">
    <div class="container header__inner">
      <a class="logo" href="#">🛍️ Vue<span>Shop</span></a>
      <div class="header__cart">Товаров в корзине: <b>{{ count }}</b></div>
    </div>
  </header>

  <main class="container main">
    <!-- ЛЕВАЯ КОЛОНКА: каталог -->
    <div class="catalog">
      <div class="filters">
        <input
          v-model="search"
          class="filters__search"
          type="search"
          placeholder="🔍 Поиск товара..."
        />

        <select v-model="category" class="filters__select">
          <option value="all">Все категории</option>
          <option v-for="item in categories" :key="item" :value="item">{{ item }}</option>
        </select>

        <select v-model="sort" class="filters__select">
          <option value="default">Без сортировки</option>
          <option value="asc">Цена: по возрастанию</option>
          <option value="desc">Цена: по убыванию</option>
        </select>
      </div>

      <p class="catalog__counter">Найдено товаров: {{ visibleProducts.length }}</p>

      <p v-if="!visibleProducts.length" class="empty">
        😕 Ничего не найдено. Попробуйте изменить запрос или категорию.
      </p>

      <div v-else class="grid">
        <ProductCard
          v-for="product in visibleProducts"
          :key="product.id"
          :product="product"
          :discount="discount"
          :in-cart="inCart(product.id)"
          @add="addToCart"
        />
      </div>
    </div>

    <!-- ПРАВАЯ КОЛОНКА: корзина -->
    <aside class="cart">
      <div class="cart__head">
        <h2>🛒 Корзина <span class="cart__count">{{ count }}</span></h2>
        <button v-if="cart.length" class="link link--danger" @click="clearCart">
          Очистить корзину
        </button>
      </div>

      <p v-if="!cart.length" class="cart__empty">Корзина пуста — добавьте товары из каталога.</p>

      <template v-else>
        <ul class="cart__list">
          <CartItem
            v-for="item in cart"
            :key="item.id"
            :item="item"
            :discount="discount"
            @increase="increase"
            @decrease="decrease"
            @remove="removeFromCart"
          />
        </ul>

        <div class="promo">
          <div class="promo__row">
            <input
              v-model="promoInput"
              type="text"
              placeholder="Промокод (WEB)"
              @keyup.enter="applyPromo"
            />
            <button class="btn btn--small" @click="applyPromo">OK</button>
          </div>
          <p v-if="promoError" class="promo__error">{{ promoError }}</p>
          <p v-else-if="discount" class="promo__ok">
            Промокод <b>{{ promo }}</b> применён: −{{ Math.round(discount * 100) }}%
            <button class="link" @click="resetPromo">отменить</button>
          </p>
        </div>

        <div class="cart__total">
          <div v-if="discount" class="cart__row cart__row--muted">
            <span>Без скидки</span>
            <s>{{ formatPrice(totalBefore) }}</s>
          </div>
          <div v-if="discount" class="cart__row cart__row--save">
            <span>Скидка</span>
            <span>−{{ formatPrice(saved) }}</span>
          </div>
          <div class="cart__row cart__row--final">
            <span>Итого</span>
            <span>{{ formatPrice(total) }}</span>
          </div>
        </div>
      </template>
    </aside>
  </main>
</template>

<style scoped>
/* ---------- Шапка ---------- */
.header {
  background: #fff;
  border-bottom: 1px solid var(--border);
  position: sticky;
  top: 0;
  z-index: 10;
}
.header__inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-block: 16px;
}
.logo {
  font-size: 22px;
  font-weight: 700;
  text-decoration: none;
  color: var(--text);
}
.logo span {
  color: var(--accent);
}
.header__cart {
  font-size: 15px;
  color: var(--muted);
}

/* ---------- Сетка страницы ---------- */
.main {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 24px;
  align-items: start;
  padding-block: 28px 48px;
}

/* ---------- Фильтры ---------- */
.filters {
  display: grid;
  grid-template-columns: 1fr 220px 220px;
  gap: 12px;
  margin-bottom: 24px;
}
.filters__search,
.filters__select {
  padding: 12px 14px;
  border: 1px solid var(--border);
  border-radius: 10px;
  font-size: 15px;
  font-family: inherit;
  background: #fff;
  outline: none;
}
.filters__search:focus,
.filters__select:focus {
  border-color: var(--accent);
}
.catalog__counter {
  color: var(--muted);
  font-size: 14px;
  margin: 0 0 14px;
}

/* ---------- Каталог ---------- */
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 16px;
}
.empty {
  text-align: center;
  padding: 48px 16px;
  color: var(--muted);
  background: #fff;
  border: 1px dashed var(--border);
  border-radius: 14px;
}

/* ---------- Корзина ---------- */
.cart {
  background: #fff;
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 18px;
  position: sticky;
  top: 88px;
}
.cart__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 14px;
}
.cart__head h2 {
  font-size: 18px;
  margin: 0;
}
.cart__count {
  display: inline-grid;
  place-items: center;
  min-width: 22px;
  height: 22px;
  padding: 0 6px;
  border-radius: 999px;
  background: var(--accent);
  color: #fff;
  font-size: 12px;
  vertical-align: middle;
}
.cart__empty {
  color: var(--muted);
  font-size: 14px;
  margin: 0;
}
.cart__list {
  list-style: none;
  margin: 0 0 14px;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-height: 340px;
  overflow-y: auto;
}

/* ---------- Промокод ---------- */
.promo__row {
  display: flex;
  gap: 8px;
  margin-bottom: 6px;
}
.promo__row input {
  flex-grow: 1;
  min-width: 0;
  padding: 9px 12px;
  border: 1px solid var(--border);
  border-radius: 8px;
  font-family: inherit;
  font-size: 14px;
  outline: none;
}
.promo__row input:focus {
  border-color: var(--accent);
}
.promo__error {
  color: var(--danger);
  font-size: 13px;
  margin: 0 0 8px;
}
.promo__ok {
  color: var(--success);
  font-size: 13px;
  margin: 0 0 8px;
}

/* ---------- Итоги ---------- */
.cart__total {
  border-top: 1px solid var(--border);
  padding-top: 12px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.cart__row {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
}
.cart__row--muted {
  color: var(--muted);
}
.cart__row--save {
  color: var(--success);
}
.cart__row--final {
  font-size: 18px;
  font-weight: 700;
  margin-top: 4px;
}

/* ---------- Адаптив ---------- */
@media (max-width: 960px) {
  .main {
    grid-template-columns: 1fr;
  }
}
@media (max-width: 760px) {
  .filters {
    grid-template-columns: 1fr;
  }
}
</style>
