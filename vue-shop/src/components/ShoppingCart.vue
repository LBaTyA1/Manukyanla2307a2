<script setup>
import { ref, watch } from 'vue'
import { useCart, formatPrice } from '@/composables/useCart'

const {
  items,
  promo,
  discount,
  totalBefore,
  total,
  saved,
  count,
  removeFromCart,
  increase,
  decrease,
  clearCart,
  applyPromo,
  resetPromo,
} = useCart()

const promoInput = ref(promo.value)
const promoError = ref('')

// Если промокод сбросили снаружи (например, очистили корзину) — чистим поле
watch(promo, (value) => {
  if (!value) promoInput.value = ''
})

function onApplyPromo() {
  promoError.value = ''
  if (!promoInput.value.trim()) {
    promoError.value = 'Введите промокод'
    return
  }
  if (!applyPromo(promoInput.value)) {
    promoError.value = 'Такого промокода не существует'
  }
}

function onResetPromo() {
  resetPromo()
  promoInput.value = ''
  promoError.value = ''
}
</script>

<template>
  <aside class="cart">
    <div class="cart__head">
      <h2>🛒 Корзина <span class="cart__count">{{ count }}</span></h2>
      <button v-if="items.length" class="link link--danger" @click="clearCart">
        Очистить корзину
      </button>
    </div>

    <p v-if="!items.length" class="cart__empty">Корзина пуста — добавьте товары из каталога.</p>

    <template v-else>
      <ul class="cart__list">
        <li v-for="item in items" :key="item.id" class="cart__item">
          <span class="cart__emoji">{{ item.image }}</span>

          <div class="cart__info">
            <span class="cart__name">{{ item.name }}</span>
            <span class="cart__price">
              {{ formatPrice(Math.round(item.price * (1 - discount))) }} × {{ item.qty }}
            </span>
          </div>

          <div class="cart__qty">
            <button @click="decrease(item.id)">−</button>
            <span>{{ item.qty }}</span>
            <button @click="increase(item.id)">+</button>
          </div>

          <button class="cart__remove" title="Удалить" @click="removeFromCart(item.id)">✕</button>
        </li>
      </ul>

      <div class="promo">
        <div class="promo__row">
          <input
            v-model="promoInput"
            type="text"
            placeholder="Промокод (WEB)"
            @keyup.enter="onApplyPromo"
          />
          <button class="btn btn--small" @click="onApplyPromo">OK</button>
        </div>
        <p v-if="promoError" class="promo__error">{{ promoError }}</p>
        <p v-else-if="discount" class="promo__ok">
          Промокод <b>{{ promo }}</b> применён: −{{ Math.round(discount * 100) }}%
          <button class="link" @click="onResetPromo">отменить</button>
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
</template>

<style scoped>
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
.cart__item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--border);
}
.cart__emoji {
  font-size: 24px;
}
.cart__info {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  min-width: 0;
}
.cart__name {
  font-size: 14px;
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.cart__price {
  font-size: 13px;
  color: var(--muted);
}
.cart__qty {
  display: flex;
  align-items: center;
  gap: 6px;
}
.cart__qty button {
  width: 24px;
  height: 24px;
  border: 1px solid var(--border);
  background: var(--bg);
  border-radius: 6px;
  cursor: pointer;
  font-size: 15px;
  line-height: 1;
}
.cart__qty button:hover {
  border-color: var(--accent);
}
.cart__remove {
  border: none;
  background: none;
  color: var(--muted);
  cursor: pointer;
  font-size: 15px;
  padding: 2px;
}
.cart__remove:hover {
  color: var(--danger);
}
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
</style>
