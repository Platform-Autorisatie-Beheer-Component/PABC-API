<template>
  <details>
    <summary>{{ itemNamePlural }}</summary>

    <alert-inline v-if="listError">{{ listError }}</alert-inline>

    <template v-else>
      <p>
        <button type="button" class="button secondary" @click="openCreateDialog">
          <icon-container icon="plus" /> Toevoegen
        </button>
      </p>

      <small-spinner v-if="listLoading" />

      <p v-else-if="!items.length">Geen {{ itemNamePlural }} gevonden.</p>

      <item-list v-else :items="items" @update="openUpdateDialog" @delete="openDeleteDialog">
        <template #item="{ item }">
          <slot name="item" :item="item"></slot>
        </template>
      </item-list>
    </template>
  </details>

  <form-modal
    :is-open="isFormDialogOpen"
    :submit-type="!form.id ? `create` : `update`"
    :loading="formLoading"
    :invalid="formInvalid"
    @submit="handleSubmit"
    @cancel="handleCancel"
  >
    <slot name="form" :form="form"></slot>
  </form-modal>

  <form-modal
    :is-open="isConfirmDialogOpen"
    submit-type="delete"
    :loading="formLoading"
    @submit="handleDelete"
    @cancel="confirmDialog.cancel"
  >
    <h2>{{ itemNameSingular }} verwijderen</h2>

    <p>
      Weet je zeker dat je
      <span class="lowercase">{{ itemNameSingular }}</span> <em>'{{ form.name }}'</em>
      wilt verwijderen? Deze actie kan niet ongedaan gemaakt worden.
    </p>
  </form-modal>
</template>

<script setup lang="ts" generic="T extends Item">
import { onMounted, ref, type DeepReadonly } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import ItemList from "@/components/ItemList.vue";
import type { Item, PabcService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";
import { useItem } from "@/composables/use-item";
import { useDialog } from "@/composables/use-dialog";

const { pabcService, itemNameSingular, itemNamePlural } = defineProps<{
  pabcService: PabcService<T>;
  itemNameSingular: string;
  itemNamePlural: string;
}>();

const form = ref<T>({} as T);

const formDialog = useDialog();
const { isOpen: isFormDialogOpen } = formDialog;

const confirmDialog = useDialog();
const { isOpen: isConfirmDialogOpen } = confirmDialog;

const {
  items,
  loading: listLoading,
  error: listError,
  fetchItems
} = useItemList(pabcService, itemNamePlural);

const {
  loading: formLoading,
  invalid: formInvalid,
  submitItem,
  deleteItem,
  clearInvalid
} = useItem(pabcService, itemNameSingular);

const getItem = (id: string) => (items.value as DeepReadonly<T[]>).find((i) => i.id === id);

const openCreateDialog = () => {
  form.value = {};

  formDialog.open();
};

const openUpdateDialog = (id: string) => {
  form.value = getItem(id);

  formDialog.open();
};

const openDeleteDialog = (id: string) => {
  form.value = getItem(id);

  confirmDialog.open();
};

const handleSubmit = async () => {
  try {
    await submitItem(form.value);

    formDialog.confirm();

    fetchItems();
  } catch {
    // Error displayed via formInvalid, keep dialog open
  }
};

const handleCancel = () => {
  clearInvalid();

  formDialog.cancel();
};

const handleDelete = async () => {
  const { id } = form.value as T;

  if (id) await deleteItem(id);

  confirmDialog.confirm();

  fetchItems();
};

onMounted(() => fetchItems());
</script>

<style lang="scss" scoped>
.lowercase {
  text-transform: lowercase;
}
</style>
