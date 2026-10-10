<script setup>
import ProductCard from './ProductCard.vue'

defineProps({
  products: { type: Array, required: true },
  discount: { type: Number, default: 0 },
  isInCart: { type: Function, required: true },
})

defineEmits(['add'])
</script>

<template>
  <section>
    <p v-if="!products.length" class="empty">
      😕 Ничего не найдено. Попробуйте изменить запрос или категорию.
    </p>

    <div v-else class="grid">
      <ProductCard
        v-for="product in products"
        :key="product.id"
        :product="product"
        :discount="discount"
        :in-cart="isInCart(product.id)"
        @add="$emit('add', $event)"
      />
    </div>
  </section>
</template>

<style scoped>
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
</style>
