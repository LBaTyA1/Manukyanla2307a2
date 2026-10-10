<script setup>
defineProps({
  count: { type: Number, required: true },
})
</script>

<template>
  <header class="header">
    <div class="container header__inner">
      <a class="logo" href="#">🛍️ Vue<span>Shop</span></a>
      <div class="header__cart">
        Товаров в корзине: <b>{{ count }}</b>
      </div>
    </div>
  </header>
</template>

<style scoped>
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
</style>
