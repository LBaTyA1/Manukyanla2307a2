<script setup>
import { ref, computed } from 'vue'
import { products } from '@/data/products'
import { useCart } from '@/composables/useCart'
import TheHeader from '@/components/TheHeader.vue'
import FiltersPanel from '@/components/FiltersPanel.vue'
import ProductList from '@/components/ProductList.vue'
import ShoppingCart from '@/components/ShoppingCart.vue'

const { count, discount, addToCart, inCart } = useCart()

const search = ref('')
const category = ref('all')
const sort = ref('default')

// Список категорий собираем из самих товаров, чтобы не дублировать данные
const categories = computed(() => [...new Set(products.map((p) => p.category))])

// Поиск + фильтр по категории + сортировка
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
  if (sort.value === 'name') result = [...result].sort((a, b) => a.name.localeCompare(b.name, 'ru'))

  return result
})
</script>

<template>
  <TheHeader :count="count" />

  <main class="container main">
    <div class="catalog">
      <FiltersPanel
        v-model:search="search"
        v-model:category="category"
        v-model:sort="sort"
        :categories="categories"
      />

      <p class="catalog__counter">Найдено товаров: {{ visibleProducts.length }}</p>

      <ProductList
        :products="visibleProducts"
        :discount="discount"
        :is-in-cart="inCart"
        @add="addToCart"
      />
    </div>

    <ShoppingCart />
  </main>
</template>

<style scoped>
.main {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 24px;
  align-items: start;
  padding-block: 28px 48px;
}
.catalog__counter {
  color: var(--muted);
  font-size: 14px;
  margin: 0 0 14px;
}
@media (max-width: 960px) {
  .main {
    grid-template-columns: 1fr;
  }
}
</style>
