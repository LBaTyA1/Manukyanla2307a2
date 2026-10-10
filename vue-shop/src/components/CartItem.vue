<script setup>
import { computed } from 'vue'
import { formatPrice } from '../data/products.js'

const props = defineProps({
  item: { type: Object, required: true },   // товар + поле qty (количество)
  discount: { type: Number, default: 0 },
})

defineEmits(['increase', 'decrease', 'remove'])

// Цена одной штуки с учётом промокода
const itemPrice = computed(() => Math.round(props.item.price * (1 - props.discount)))
</script>

<template>
  <li class="item">
    <span class="item__emoji">{{ item.image }}</span>

    <div class="item__info">
      <span class="item__name">{{ item.name }}</span>
      <span class="item__price">{{ formatPrice(itemPrice) }} × {{ item.qty }}</span>
    </div>

    <div class="item__qty">
      <button @click="$emit('decrease', item.id)">−</button>
      <span>{{ item.qty }}</span>
      <button @click="$emit('increase', item.id)">+</button>
    </div>

    <button class="item__remove" title="Удалить" @click="$emit('remove', item.id)">✕</button>
  </li>
</template>

<style scoped>
.item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--border);
}
.item__emoji {
  font-size: 24px;
}
.item__info {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  min-width: 0;
}
.item__name {
  font-size: 14px;
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.item__price {
  font-size: 13px;
  color: var(--muted);
}
.item__qty {
  display: flex;
  align-items: center;
  gap: 6px;
}
.item__qty button {
  width: 24px;
  height: 24px;
  border: 1px solid var(--border);
  background: var(--bg);
  border-radius: 6px;
  cursor: pointer;
  font-size: 15px;
  line-height: 1;
}
.item__qty button:hover {
  border-color: var(--accent);
}
.item__remove {
  border: none;
  background: none;
  color: var(--muted);
  cursor: pointer;
  font-size: 15px;
  padding: 2px;
}
.item__remove:hover {
  color: var(--danger);
}
</style>
