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

      <p v-else-if="!items.length">
        Geen <span class="lowercase">{{ itemNamePlural }}</span> gevonden.
      </p>

      <template
        v-else-if="groupedApplicationRoles.length"
        v-for="{ application, roles } in groupedApplicationRoles"
        :key="application"
      >
        <h3>{{ application }}</h3>

        <item-list :items="roles" @update="openUpdateDialog" @delete="openDeleteDialog">
          <template #item="{ item }">
            <slot name="item" :item="item"></slot>
          </template>
        </item-list>
      </template>

      <item-list v-else :items="items" @update="openUpdateDialog" @delete="openDeleteDialog">
        <template #item="{ item }">
          <slot name="item" :item="item"></slot>
        </template>
      </item-list>
    </template>
  </details>

  <form-modal
    ref="form-dialog"
    :submit-type="!form.id ? `create` : `update`"
    @submit="handleSubmit"
  >
    <slot name="form" :form="form"></slot>
  </form-modal>

  <form-modal ref="confirm-dialog" submit-type="delete" @submit="handleDelete">
    <h2>{{ itemNameSingular }} verwijderen</h2>

    <p>
      Weet je zeker dat je
      <span class="lowercase">{{ itemNameSingular }}</span> <em>'{{ form.name }}'</em>
      wilt verwijderen? Deze actie kan niet ongedaan gemaakt worden.
    </p>
  </form-modal>
</template>

<script setup lang="ts" generic="T extends Item">
import { computed, onMounted, ref, useTemplateRef, type DeepReadonly } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import ItemList from "@/components/ItemList.vue";
import type { ApplicationRole, Item, PabcService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";
import { useItem } from "@/composables/use-item";

const { pabcService, itemNameSingular, itemNamePlural } = defineProps<{
  pabcService: PabcService<T>;
  itemNameSingular: string;
  itemNamePlural: string;
}>();

const form = ref<T>({} as T);

const formDialog = useTemplateRef("form-dialog");
const confirmDialog = useTemplateRef("confirm-dialog");

const {
  items,
  loading: listLoading,
  error: listError,
  fetchItems
} = useItemList(pabcService, itemNamePlural);

const { submitItem, deleteItem } = useItem(pabcService, itemNameSingular);

const getItem = (id: string) => (items.value as DeepReadonly<T[]>).find((i) => i.id === id);

const openCreateDialog = () => {
  form.value = {};

  formDialog.value?.open();
};

const openUpdateDialog = (id: string) => {
  form.value = getItem(id);

  formDialog.value?.open();
};

const openDeleteDialog = (id: string) => {
  form.value = getItem(id);

  confirmDialog.value?.open();
};

const handleSubmit = async () => {
  await submitItem(form.value);
  fetchItems();
};

const handleDelete = async () => {
  const { id } = form.value as T;

  if (id) await deleteItem(id);

  fetchItems();
};

const groupedApplicationRoles = computed(() => {
  if (!(items.value.length && "applicationId" in items.value[0])) return [];

  return Object.entries(
    (items.value as DeepReadonly<ApplicationRole[]>).reduce(
      (acc, item) => ({ ...acc, [item.application]: [...(acc[item.application] || []), item] }),
      {} as Record<string, ApplicationRole[]>
    )
  ).map(([application, roles]) => ({
    application,
    roles
  }));
});

onMounted(() => fetchItems());
</script>

<style lang="scss" scoped>
h3 {
  font-size: inherit;
  margin-block-start: var(--spacing-large);
  margin-block-end: var(--spacing-default);
}

.lowercase {
  text-transform: lowercase;
}
</style>
