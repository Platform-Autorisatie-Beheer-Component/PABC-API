<template>
  <details>
    <summary>{{ itemNamePlural }}</summary>

    <alert-inline v-if="listError">{{ listError }}</alert-inline>

    <template v-else>
      <p>
        <button type="button" class="button secondary" @click="handleCreate">
          <icon-container icon="plus" /> Toevoegen
        </button>
      </p>

      <small-spinner v-if="listLoading" />

      <p v-else-if="!items.length">Geen {{ itemNamePlural }} gevonden.</p>

      <ul v-else class="reset">
        <li v-for="item in items" :key="item.id">
          <section>
            <slot name="item" :item="item"></slot>
          </section>

          <menu class="reset">
            <li>
              <button type="button" class="button secondary" @click="handleUpdate(item.id)">
                <icon-container icon="pen" />

                <span class="visually-hidden">Item aanpassen</span>
              </button>
            </li>

            <li>
              <button type="button" class="button danger" @click="handleDelete(item.id)">
                <icon-container icon="trash" />

                <span class="visually-hidden">Item verwijderen</span>
              </button>
            </li>
          </menu>
        </li>
      </ul>
    </template>
  </details>

  <form-modal
    :is-open="isFormDialogOpen"
    :submit-type="!form.id ? `create` : `update`"
    :loading="formLoading"
    :error="formError"
    :invalid="formInvalid"
    @submit="submit"
    @cancel="cancel"
  >
    <slot name="form" :form="form"></slot>
  </form-modal>

  <form-modal
    :is-open="isConfirmDialogOpen"
    submit-type="delete"
    :loading="formLoading"
    :error="formError"
    @submit="remove"
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
  item,
  form,
  loading: formLoading,
  error: formError,
  invalid: formInvalid,
  fetchItem,
  submitItem,
  deleteItem
} = useItemForm(pabcService, itemNameSingular);

const handleCreate = async () => {
  item.value = null;

  formDialog.open();
};

const handleUpdate = async (id?: string) => {
  if (!id) return;

  fetchItem(id);

  formDialog.open();
};

const submit = async () => {
  try {
    await submitItem();

    formDialog.confirm();

    fetchItems();
  } catch {}
};

const cancel = async () => {
  formError.value = "";
  formInvalid.value = "";

  formDialog.confirm();
};

const handleDelete = async (id?: string) => {
  if (!id) return;

  fetchItem(id);

  confirmDialog.open();
};

const remove = async () => {
  await deleteItem();

  confirmDialog.cancel();

  fetchItems();
};

onMounted(() => fetchItems());
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

details > p {
  margin-block: var(--spacing-default);
}

ul > li {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
  padding-block-start: var(--spacing-small);
  margin-block-end: var(--spacing-small);
  border-block-start: 1px solid var(--secondary);

  @media (min-width: variables.$breakpoint-md) {
    & {
      flex-direction: row;
      align-items: flex-end;
    }
  }

  section {
    flex: 1;
    display: flex;
    flex-direction: column;
    row-gap: var(--spacing-small);
  }

  menu {
    display: flex;
    column-gap: var(--spacing-small);

    button {
      margin-block-end: 0;
    }
  }
}

.lowercase {
  text-transform: lowercase;
}
</style>
