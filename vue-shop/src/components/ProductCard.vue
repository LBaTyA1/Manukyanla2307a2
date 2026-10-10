<script setup>
import { computed } from 'vue'
import { formatPrice } from '@/composables/useCart'

const props = defineProps({
  product: { type: Object, required: true },
  discount: { type: Number, default: 0 },
  inCart: { type: Boolean, default: false },
})

defineEmits(['add'])

// Если в image лежит путь к файлу (/img/..., .png, http...) — показываем <img>,
// иначе рисуем эмодзи как текст.
const isImageFile = computed(() => /^(https?:|\/|\.)|\.(png|jpe?g|svg|webp|gif)$/i.test(props.product.image))

const finalPrice = computed(() => Math.round(props.product.price * (1 - props.discount)))
</script>

<template>
  <article class="card">
    <div class="card__image">
      <img v-if="isImageFile" :src="product.image" :alt="product.name" />
      <span v-else>{{ product.image }}</span>
      <span v-if="discount" class="card__badge">-{{ Math.round(discount * 100) }}%</span>
    </div>

    <span class="card__category">{{ product.category }}</span>
    <h3 class="card__name">{{ product.name }}</h3>

    <div class="card__prices">
      <span class="card__price">{{ formatPrice(finalPrice) }}</span>
      <s v-if="discount" class="card__old">{{ formatPrice(product.price) }}</s>
    </div>

    <button class="btn" :class="{ 'btn--ghost': inCart }" @click="$emit('add', product)">
      {{ inCart ? '✓ В корзине (ещё)' : 'В корзину' }}
    </button>
  </article>
</template>

<style scoped>
.card {
  background: #fff;
  border: 1px solid var(--border);
  border-radius: 14px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  transition: box-shadow 0.2s, transform 0.2s;
}
.card:hover {
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}
.card__image {
  position: relative;
  height: 130px;
  display: grid;
  place-items: center;
  font-size: 64px;
  background: var(--bg);
  border-radius: 10px;
  margin-bottom: 4px;
}
.card__image img {
  max-height: 100%;
  max-width: 100%;
  object-fit: contain;
}
.card__badge {
  position: absolute;
  top: 8px;
  right: 8px;
  background: var(--danger);
  color: #fff;
  font-size: 12px;
  font-weight: 700;
  padding: 3px 8px;
  border-radius: 999px;
}
.card__category {
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: var(--muted);
}
.card__name {
  font-size: 16px;
  margin: 0;
  flex-grow: 1;
}
.card__prices {
  display: flex;
  align-items: baseline;
  gap: 8px;
}
.card__price {
  font-size: 19px;
  font-weight: 700;
}
.card__old {
  color: var(--muted);
  font-size: 14px;
}
</style>
