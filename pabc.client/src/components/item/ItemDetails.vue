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
    :error="formError"
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
    :error="formError"
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
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import ItemList from "@/components/ItemList.vue";
import type { Item, PabcService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";
import { useItemForm } from "@/composables/use-item-form";
import { useDialog } from "@/composables/use-dialog";

const { pabcService, itemNameSingular, itemNamePlural } = defineProps<{
  pabcService: PabcService<T>;
  itemNameSingular: string;
  itemNamePlural: string;
}>();

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
  form,
  loading: formLoading,
  error: formError,
  invalid: formInvalid,
  fetchItem,
  submitItem,
  deleteItem,
  clearItem
} = useItemForm(pabcService, itemNameSingular);

const openCreateDialog = () => {
  clearItem();

  formDialog.open();
};

const openUpdateDialog = (id: string) => {
  fetchItem(id);

  formDialog.open();
};

const openDeleteDialog = (id: string) => {
  fetchItem(id);

  confirmDialog.open();
};

const handleSubmit = async () => {
  try {
    await submitItem();

    formDialog.confirm();

    fetchItems();
  } catch {
    // Error displayed via formInvalid, keep dialog open
  }
};

const handleCancel = () => {
  clearItem();

  formDialog.cancel();
};

const handleDelete = async () => {
  await deleteItem();

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
